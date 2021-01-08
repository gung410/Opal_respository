using Communication.Business.HttpClients;
using Communication.Business.Models;
using Communication.Business.Models.Command;
using Communication.Business.Models.Email;
using Communication.Business.Services;
using Communication.Business.Services.Email;
using Communication.DataAccess.Notification;
using cx.datahub.scheduling.jobs.shared;
using Hangfire.Server;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Communication.Api.Hangfire
{
    public class DigestEmailJob : IDigestEmailJob
    {
        private readonly IEnumerable<ICommunicationService> _communicationServiceResolver;
        private readonly ILogger<DigestEmailJob> _logger;
        private readonly IConfiguration _configuration;
        private readonly IOrganizationClientService _organizationClientService;
        private readonly IUserNotificationSettingRepository _userNotificationSettingRepository;
        private readonly ConnectionFactory _factory;
        private readonly IConnection _connection;

        public DigestEmailJob(IEnumerable<ICommunicationService> communicationServiceResolver, ILogger<DigestEmailJob> logger, IConfiguration configuration,
            IOrganizationClientService organizationClientService, ConnectionFactory factory, IConnection connection, IUserNotificationSettingRepository userNotificationSettingRepository)
        {
            _communicationServiceResolver = communicationServiceResolver;
            _logger = logger;
            _configuration = configuration;
            _organizationClientService = organizationClientService;
            _factory = factory;
            _connection = connection;
            _userNotificationSettingRepository = userNotificationSettingRepository;
        }
        public void Execute(PerformContext performContext, params object[] inputs)
        {
            throw new NotImplementedException();
        }


        private EmailQueueMessage InitNotificationMessage(List<NotificationDigestModel> notificationDigestModels, OrgnanizationResponseDto users, int count)
        {
            var user = users.Items.FirstOrDefault();
            var data = new EmailQueueMessage
            {
                Payload = new Business.Models.Payload(),
                Created = DateTime.UtcNow,
                Routing = new Business.Models.Routing
                {
                    Action = "communication_api.communication.send.email.requested"
                },
                Type = "command",
                Version = "1.0"
            };

            data.Payload.Body = new EmailMessageBody
            {
                Channel = Channel.Email,
                Subject = $"OPAL2.0 - You have {count} new message(s) for yesterday",
                //DisplayMessage = $"OPAL2.0 - You have {count} new message(s) for yesterday",
                IsHtmlEmail = true,
                Emails = new HashSet<string> { user.EmailAddress },
                TemplateData = new TemplateData
                {
                    TemplateName = "DigestEmail",
                    Module = "Digest",
                    Project = "Opal",
                    Data = new Dictionary<string, object>
                    {
                        {"Count", notificationDigestModels.Count },
                        {"Notifications", notificationDigestModels },
                        { "UserData" , new {RecipientName = user.FirstName }}
                    }
                }
            };
            return data;
        }

        private async Task PublishMessageToQueue(QueueMessageBase message)
        {

            var json = JsonConvert.SerializeObject(message, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            _logger.LogInformation($"Send to Communication.Sender: {json}");
            _factory.RequestedHeartbeat = TimeSpan.FromSeconds(60);
            var exchangeName = _configuration["COMMAND_EXCHANGE_NAME"];
            var publishChannel = _connection.CreateModel();
            var body = Encoding.UTF8.GetBytes(json);
            publishChannel.BasicPublish(exchange: exchangeName,
                                     routingKey: message.Routing.Action,
                                     basicProperties: null,
                                     body: body);
            publishChannel.Close();
            await Task.CompletedTask;
        }

        public async Task ExecuteTask(PerformContext performContext, params object[] inputs)
        {
            var emailService = _communicationServiceResolver.FirstOrDefault(x => x.GetType() == typeof(EmailService));
            TimeZoneInfo timeZoneInfo;
            try
            {
                timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("Asia/Singapore");
            }
            catch (TimeZoneNotFoundException)
            {
                timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("Singapore Standard Time");
            }
            var todaySG = DateTime.UtcNow.AddHours(timeZoneInfo.BaseUtcOffset.Hours).Date;
            var yesterdayStartSg = todaySG.AddHours(-timeZoneInfo.BaseUtcOffset.Hours).AddDays(-1);
            var yesterdayEndSg = todaySG.AddHours(-timeZoneInfo.BaseUtcOffset.Hours).AddSeconds(-1);
            var digestInfo = await emailService.GetDigestNotification(yesterdayStartSg, yesterdayEndSg);
            foreach (var item in digestInfo)
            {
                _logger.LogInformation($"Start sending digest email for userId: {item.Key}");
                var users = await _organizationClientService.GetUsers(_configuration["cxToken"], userIds: new HashSet<string> { item.Key });
                var email = InitNotificationMessage(item.Value, users, item.Value.Count);
                await PublishMessageToQueue(email);
                _logger.LogInformation($"Finished sending digest email for userId: {item.Key}");

            }
        }
    }
}
