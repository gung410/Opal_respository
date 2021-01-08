using Communication.Api.Extensions;
using Communication.Business.Extensions;
using Communication.Business.HttpClients;
using Communication.Business.Models;
using Communication.Business.Models.Command;
using Communication.Business.Models.Email;
using Communication.Business.Models.FirebaseCloudMessage;
using Communication.Business.Services;
using Communication.Business.Services.Email;
using Communication.Business.Services.FirebaseCloudMessage;
using Communication.DataAccess.Notification;
using Datahub.Processor.Base.ProcessorRegister;
using Datahub.Processor.Base.Settings;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Communication.Api.Processor
{
    public class CommunicationHandler : ActionHandlerBase, IActionHandler
    {
        private IEnumerable<ICommunicationService> _communicationServices;
        private IIdentityServerClientService _identityServerClientService;
        private IConfiguration _configuration;
        private IWebHostEnvironment _hostingEnvironment;
        private INotificationReferenceRepository _notificationReferenceRepository;
        private IUserNotificationSettingRepository _userNotificationSettingRepository;
        private IOrganizationClientService _organizationClientService;
        private INotificationHistoryRepository _notificationHistoryRepository;
        private INotificationRepository _notificationRepository;
        private readonly IServiceProvider _serviceProvider;
        private IConnection _connection;
        private ConnectionFactory _factory;
        private const int pageSize = 100;
        public CommunicationHandler(ILogger<CommunicationHandler> logger, IServiceProvider serviceProvider) : base(logger)
        {
            _serviceProvider = serviceProvider;
        }

        public override string Action => ActionConstants.AcceptAll;

        public override Func<dynamic, Task> Handler => async dynamicObject =>
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                _communicationServices = scope.ServiceProvider.GetServices<ICommunicationService>();
                _notificationReferenceRepository = scope.ServiceProvider.GetService<INotificationReferenceRepository>();
                _notificationHistoryRepository = scope.ServiceProvider.GetService<INotificationHistoryRepository>();
                _notificationRepository = scope.ServiceProvider.GetService<INotificationRepository>();
                _identityServerClientService = scope.ServiceProvider.GetService<IIdentityServerClientService>();
                _organizationClientService = scope.ServiceProvider.GetService<IOrganizationClientService>();
                _configuration = scope.ServiceProvider.GetService<IConfiguration>();
                _hostingEnvironment = scope.ServiceProvider.GetService<IWebHostEnvironment>();
                _connection = scope.ServiceProvider.GetService<IConnection>();
                _factory = scope.ServiceProvider.GetService<ConnectionFactory>();
                _userNotificationSettingRepository = scope.ServiceProvider.GetService<IUserNotificationSettingRepository>();
            }
            if (dynamicObject.Type != "command")
                return;
            string messageStr = JsonConvert.SerializeObject(dynamicObject, new JsonSerializerSettings
            {
                ContractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new CustomNamingStrategy()
                }
            });
            var message = JsonConvert.DeserializeObject<CommunicationCommand>(messageStr);

            if (message.Payload.Body.Channel.HasValue && message.Payload.Body.Message.MessageType != DataAccess.MessageType.Banner)
            {
                await HandleCommunicationCommandAsync(message);
            }
            else
            {
                await HandleCommunicationCommandAsyncDefaultChannel(message);
            }


        };

        private async Task ProcessSendNotificationByDate(DateTime? date, string cxToken)
        {
            date = date ?? DateTime.UtcNow.Date;

            var begin = date.Value.Date;
            var end = begin.AddDays(1);

            var notificationInCurrentDate = await _notificationRepository.GetAllAsync(x => x.StartDateUtc.HasValue && x.StartDateUtc.Value >= begin && x.StartDateUtc.Value < end);
            var notificationId = notificationInCurrentDate.Select(x => x.Id);
            var notificationHistories = await _notificationHistoryRepository.GetAllAsync(x => x.Cancelled == false && notificationId.Contains(x.NotificationId));
            if (notificationHistories.Any())
                _logger.LogInformation($"Scheduled messsage(s) found, Started sending scheduled notification message for {notificationHistories.Count} user(s)");
            foreach (var item in notificationHistories)
            {
                var notification = notificationInCurrentDate.FirstOrDefault(x => x.Id == item.NotificationId);
                var uid = Guid.NewGuid().ToString();
                CommunicationCommand message = new CommunicationCommand
                {
                    Routing = new Business.Models.Command.Routing
                    {
                        Entity = "communication_api.notification_by_date.message",
                        EntityId = uid,
                        Action = "communication_api.communication.send.notification.success",
                        ActionVersion = string.Empty
                    },
                    Type = "command",
                    Version = "1.0",
                    Created = DateTime.UtcNow,
                    Payload = new Business.Models.Command.Payload
                    {
                        Identity = new Business.Models.Command.Identity
                        {
                            ClientId = "communication_api",
                            CustomerId = string.Empty,
                            UserId = uid
                        },
                        References = new Business.Models.Command.References
                        {
                            CorrelationId = uid,
                        },
                        Body = new Body
                        {
                            Message = new Message
                            {
                                Subject = notification.DefaultSubject,
                                DisplayMessage = notification.DefaultBody
                            },
                            Recipient = new Recipient
                            {
                                UserIds = new HashSet<string> { item.UserId }
                            }
                        }
                    }
                };
                await HandleCommunicationCommandAsync(message);
                if (notificationHistories.Any())
                    _logger.LogInformation($"Finish sending scheduled messsage(s) for {notificationHistories.Count} user(s)");
            }
        }

        private async Task HandleCommunicationCommandAsyncDefaultChannel(CommunicationCommand communicationCommand)
        {
            var communicationService = _communicationServices.FirstOrDefault(x => x.GetType().Name == typeof(FcmPushNotificationService).Name);
            if (communicationCommand.Payload.Body.Message.IsGlobal.HasValue && communicationCommand.Payload.Body.Message.IsGlobal.Value
                && communicationCommand.Payload.Body.Message.MessageType == DataAccess.MessageType.Banner 
                && communicationCommand.Payload.Body.Channel == null)
            {
                _logger.LogInformation($"Started sending GLOBAL message...");
                Notification notificationModel = await CreateNotificationWithHistory(communicationCommand, new HashSet<string>());
                _logger.LogInformation($"Finished sending GLOBAL message.");
                return;
            }
            if (communicationCommand.Payload.Body.Message.IsGlobal.HasValue && communicationCommand.Payload.Body.Message.IsGlobal.Value
                && communicationCommand.Payload.Body.Message.MessageType == DataAccess.MessageType.Banner
                && communicationCommand.Payload.Body.Channel == Channel.Email)
            {
                int pageIndex = 1;
                var userIds = communicationCommand.Payload.Body.Recipient.UserIds.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToHashSet();
                var hasMore = userIds.Count() > 0;
                var createdHistories = new HashSet<string>();
                while (hasMore)
                {
                    _logger.LogInformation($"Started getting user info for {userIds.Count} user(s)");
                    var users = await _organizationClientService.GetUsers(_configuration["cxToken"], userIds: userIds);
                    var channelGroups = await InitChannelGroups(communicationCommand, userIds);

                    foreach (var item in channelGroups)
                    {
                        ISet<string> tokens = new HashSet<string>();
                        if (item.Key != Channel.Email && item.Key != Channel.SMS)
                        {
                            tokens = await GetInstanceIdTokens(item.Value);
                        }
                        if (item.Key == Channel.Email || item.Key == Channel.Banner || tokens.Any())
                        {
                            _logger.LogInformation($"Started sending notification message to queue for {userIds.Count} user(s)");
                            var needCreateHistories = item.Value.Where(x => !createdHistories.Contains(x)).ToHashSet();
                            Notification notificationModel = await CreateNotificationWithHistory(communicationCommand, needCreateHistories);
                            foreach (var userid in item.Value)
                            {
                                createdHistories.Add(userid);
                            }
                            if (item.Key != Channel.Banner)
                            {
                                var notificationMessage = InitNotificationMessage(communicationCommand, item, notificationModel.Id.ToString(), tokens, users, null);
                                await PublishMessageToQueue(notificationMessage);
                            }
                            _logger.LogInformation($"Finished sending notification for {userIds.Count} user(s). Channel: {item.Key}");
                        }
                        else
                        {
                            _logger.LogInformation($"Skipped sending notification. Could not get enough channel info. Channel: {item.Key}");
                        }
                    }
                    pageIndex++;
                    userIds = communicationCommand.Payload.Body.Recipient.UserIds.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToHashSet();
                    hasMore = userIds.Count() > 0;
                }
                return;
            }
            if (communicationCommand.Payload.Body.Recipient.UserIds != null)
            {

                int pageIndex = 1;
                var userIds = communicationCommand.Payload.Body.Recipient.UserIds.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToHashSet();
                var hasMore = userIds.Count() > 0;
                var createdHistories = new HashSet<string>();
                while (hasMore)
                {
                    _logger.LogInformation($"Started getting user info for {userIds.Count} user(s)");
                    var users = await _organizationClientService.GetUsers(_configuration["cxToken"], userIds: userIds);
                    var channelGroups = await InitChannelGroups(communicationCommand, userIds);

                    foreach (var item in channelGroups)
                    {
                        ISet<string> tokens = new HashSet<string>();
                        if (item.Key != Channel.Email && item.Key != Channel.SMS)
                        {
                            tokens = await GetInstanceIdTokens(item.Value);
                        }
                        if (item.Key == Channel.Email || item.Key == Channel.Banner || tokens.Any())
                        {
                            _logger.LogInformation($"Started sending notification message to queue for {userIds.Count} user(s)");
                            var needCreateHistories = item.Value.Where(x => !createdHistories.Contains(x)).ToHashSet();
                            Notification notificationModel = await CreateNotificationWithHistory(communicationCommand, needCreateHistories);
                            foreach (var userid in item.Value)
                            {
                                createdHistories.Add(userid);
                            }
                            if (item.Key != Channel.Banner)
                            {
                                var notificationMessage = InitNotificationMessage(communicationCommand, item, notificationModel.Id.ToString(), tokens, users, null);
                                await PublishMessageToQueue(notificationMessage);
                            }
                            _logger.LogInformation($"Finished sending notification for {userIds.Count} user(s). Channel: {item.Key}");
                        }
                        else
                        {
                            _logger.LogInformation($"Skipped sending notification. Could not get enough channel info. Channel: {item.Key}");
                        }
                    }
                    pageIndex++;
                    userIds = communicationCommand.Payload.Body.Recipient.UserIds.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToHashSet();
                    hasMore = userIds.Count() > 0;
                }
                return;
            }
            if (communicationCommand.Payload.Body.Recipient.UserTypeIds != null
                || communicationCommand.Payload.Body.Recipient.DepartmentIds != null
                || communicationCommand.Payload.Body.Recipient.UserGroupIds != null
                || communicationCommand.Payload.Body.Recipient.ForExternalUsers.HasValue
                || communicationCommand.Payload.Body.Recipient.ForHrmsUsers.HasValue)
            {
                var hasMore = true;
                int pageIndex = 1;
                int total = 0;
                var createdHistories = new HashSet<string>();
                Notification notificationModel = await CreateNotification(communicationCommand);
                while (hasMore)
                {
                    var users = await _organizationClientService.GetUsers(cxToken: _configuration["cxToken"],
                        departmentIds: communicationCommand.Payload.Body.Recipient.DepartmentIds, userGroupIds: communicationCommand.Payload.Body.Recipient.UserGroupIds,
                        roles: communicationCommand.Payload.Body.Recipient.UserTypeIds, pageIndex: pageIndex,
                        forExternalUsers: communicationCommand.Payload.Body.Recipient.ForExternalUsers,
                        forHrmsUsers: communicationCommand.Payload.Body.Recipient.ForHrmsUsers);
                    var channelGroups = await InitChannelGroups(communicationCommand, users.Items.Select(x => x.Identity.ExtId).Distinct().ToHashSet());
                    foreach (var item in channelGroups)
                    {
                        var needCreateHistories = item.Value.Where(x => !createdHistories.Contains(x)).ToHashSet();
                        await CreateNotificationHistory(notificationModel.Id, needCreateHistories, communicationCommand);
                        foreach (var userid in item.Value)
                        {
                            createdHistories.Add(userid);
                        }
                        var topic = Guid.NewGuid().ToString();
                        try
                        {
                            _logger.LogInformation($"Started sending notification message to queue for {item.Value.Count} user(s). Channel: {item.Key}");
                            ISet<string> tokens = new HashSet<string>();
                            if (item.Key != Channel.Email && item.Key != Channel.SMS)
                            {
                                tokens = await GetInstanceIdTokens(item.Value);
                            }
                            if (item.Key == Channel.Email || item.Key == Channel.Banner || tokens.Any())
                            {
                                if (item.Key != Channel.Banner)
                                {
                                    var notificationMessage = InitNotificationMessage(communicationCommand, item, notificationModel.Id.ToString(), tokens, users, null);
                                    await PublishMessageToQueue(notificationMessage);
                                }
                                _logger.LogInformation($"Finished sending notification for {users.Items.Count} user(s). Channel: {item.Key}");
                            }
                            else
                            {
                                _logger.LogInformation($"Skipped sending notification. Could not get enough channel info. Channel: {item.Key}");
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, ex.Message);
                        }
                    }
                    total += users.Items.Count;
                    pageIndex++;
                    hasMore = users.HasMoreData;
                }
                return;
            }
        }

        private string GetRoutingAction(Channel key)
        {
            switch (key)
            {
                case Channel.Default:
                    {
                        return "communication_api.communication.send.notification.requested";
                    }
                case Channel.Email:
                    {
                        return "communication_api.communication.send.email.requested";
                    }
                case Channel.System:
                case Channel.SMS:
                    {
                        return "";
                    }
                default: return string.Empty;
            }
        }

        private QueueMessageBase InitNotificationMessage(CommunicationCommand communicationCommand,
            KeyValuePair<Channel, HashSet<string>> key,
            string notificationId, ISet<string> tokens,
            OrgnanizationResponseDto users, ISet<string> emails)
        {
            switch (key.Key)
            {
                case Channel.Default:
                case Channel.System:
                    {
                        var data = new FireBaseNotificationQueueMessage
                        {
                            Payload = new Business.Models.Payload(),
                            Created = DateTime.UtcNow,
                            Routing = new Business.Models.Routing
                            {
                                Action = "communication_api.communication.send.notification.requested"
                            },
                            Type = "command",
                            Version = "1.0"
                        };
                        data.Payload.Body = new NotificationMessageBody
                        {
                            RegistrationTokens = tokens,
                            DisplayMessage = !string.IsNullOrEmpty(communicationCommand.Payload.Body.Message.DisplayMessage) ? communicationCommand.Payload.Body.Message.DisplayMessage : communicationCommand.Payload.Body.Message.PlainMessage,
                            Subject = communicationCommand.Payload.Body.Message.Subject,
                            Channel = key.Key,
                            NotificationId = notificationId,
                            TemplateData = InitEmailTemplateData(communicationCommand, users != null ? users.Items : null),
                        };
                        return data;
                    }
                case Channel.Email:
                    {
                        emails = users != null ? users.Items.Where(x => key.Value.Contains(x.Identity.ExtId)).Select(x => x.EmailAddress).ToHashSet() : emails;
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

                        if (communicationCommand.Payload.Body.TemplateData == null)
                        {
                            data.Payload.Body = new EmailMessageBody
                            {
                                Channel = Channel.Email,
                                Subject = communicationCommand.Payload.Body.Message.Subject,
                                DisplayMessage = communicationCommand.Payload.Body.Message.DisplayMessage,
                                PlainMessage = communicationCommand.Payload.Body.Message.PlainMessage,
                                IsHtmlEmail = !string.IsNullOrEmpty(communicationCommand.Payload.Body.Message.DisplayMessage),
                                Emails = emails,
                                TemplateData = new TemplateData(),
                                Attachments = communicationCommand.Payload.Body.Message.Attachments ?? null
                            };
                            if (_configuration["PROJECT_NAME"].ToLower() == "opal")
                            {
                                //todo: move to default notification template config 
                                data.Payload.Body.TemplateData.Project = "Opal";
                                data.Payload.Body.TemplateData.Module = "SystemAdmin";
                                data.Payload.Body.TemplateData.TemplateName = "SystemNotification";
                                data.Payload.Body.TemplateData.Data = new Dictionary<string, object>();
                                data.Payload.Body.TemplateData.Data.Add("Title", "OPAL 2.0 System notification");
                                data.Payload.Body.TemplateData.Data.Add("Subject", communicationCommand.Payload.Body.Message.Subject);
                                data.Payload.Body.TemplateData.Data.Add("Body", communicationCommand.Payload.Body.Message.DisplayMessage);
                            }
                        }
                        else
                        {
                            data.Payload.Body = new EmailMessageBody
                            {
                                Channel = Channel.Email,
                                Subject = communicationCommand.Payload.Body.Message.Subject,
                                DisplayMessage = !string.IsNullOrEmpty(communicationCommand.Payload.Body.Message.DisplayMessage) ? communicationCommand.Payload.Body.Message.DisplayMessage : communicationCommand.Payload.Body.Message.PlainMessage,
                                IsHtmlEmail = !string.IsNullOrEmpty(communicationCommand.Payload.Body.Message.DisplayMessage),
                                Emails = emails,
                                TemplateData = InitEmailTemplateData(communicationCommand, users != null ? users.Items : null),
                                Attachments = communicationCommand.Payload.Body.Message.Attachments ?? null
                            };
                        }
                        return data;
                    }
                default: return null;
            }
        }

        private TemplateData InitEmailTemplateData(CommunicationCommand communicationCommand, List<UserDomainDto> users)
        {
            var result = new Dictionary<string, object>();
            if (communicationCommand.Payload.Body.Recipient.Emails != null && communicationCommand.Payload.Body.Recipient.Emails.Any())
            {
                return communicationCommand.Payload.Body.TemplateData;
            }
            if (users == null || !users.Any())
                return communicationCommand.Payload.Body.TemplateData;
            if (communicationCommand.Payload.Body.TemplateData.ReferenceData != null)
            {

                foreach (var item in communicationCommand.Payload.Body.TemplateData.ReferenceData)
                {
                    var user = users.FirstOrDefault(x => x.Identity.ExtId == item.Key);
                    if (user != null)
                    {
                        result.Add(user.EmailAddress.Replace(".", ""), item.Value);
                    }
                }
                communicationCommand.Payload.Body.TemplateData.ReferenceData = result;
                return communicationCommand.Payload.Body.TemplateData;
            }
            else
            {
                return communicationCommand.Payload.Body.TemplateData;
            }

        }

        private async Task<Dictionary<Channel, HashSet<string>>> InitChannelGroups(CommunicationCommand communicationCommand, ISet<string> userids)
        {
            if (communicationCommand.Payload.Body.Channel == Channel.Banner || communicationCommand.Payload.Body.Message.MessageType == DataAccess.MessageType.Banner)
            {
                var resultBanner = new Dictionary<Channel, HashSet<string>>();
                resultBanner.Add(Channel.Banner, userids.ToHashSet());
                if (communicationCommand.Payload.Body.Channel == Channel.Email)
                {
                    resultBanner.Add(Channel.Email, userids.ToHashSet());
                }
                return resultBanner;
            }
            var settings = await _userNotificationSettingRepository.GetAllAsync(x => userids.Contains(x.UserId));
            if (!settings.Any())
            {
                return new Dictionary<Channel, HashSet<string>>
                {
                    {Channel.Email, userids.ToHashSet() }
                };
            }
            var channels = settings.Select(x => x.NotificationChannel).Distinct().ToList();
            var result = new Dictionary<Channel, HashSet<string>>();
            foreach (var item in channels)
            {
                switch (item)
                {
                    case NotificationChannel.InAppNotification:
                        {
                            if (!result.ContainsKey(Channel.System))
                                result.Add(Channel.System, new HashSet<string>());
                            break;
                        }
                    case NotificationChannel.InAppNotificationAndEmail:
                        {
                            if (!result.ContainsKey(Channel.System))
                                result.Add(Channel.System, new HashSet<string>());
                            if (!result.ContainsKey(Channel.Email))
                                result.Add(Channel.Email, new HashSet<string>());
                            break;
                        }
                    case NotificationChannel.Email:
                        {
                            if (!result.ContainsKey(Channel.Email))
                                result.Add(Channel.Email, new HashSet<string>());
                            break;
                        }
                }
            }

            foreach (var item in userids)
            {
                var channelSetting = settings.FirstOrDefault(x => x.UserId == item);
                if (channelSetting == null)
                {
                    result[Channel.Email].Add(item);
                }
                else
                {
                    switch (channelSetting.NotificationChannel)
                    {
                        case NotificationChannel.InAppNotification:
                            {
                                result[Channel.System].Add(channelSetting.UserId);
                                break;
                            }
                        case NotificationChannel.InAppNotificationAndEmail:
                            {
                                result[Channel.System].Add(channelSetting.UserId);
                                result[Channel.Email].Add(channelSetting.UserId);
                                break;
                            }
                        case NotificationChannel.Email:
                            {
                                result[Channel.Email].Add(channelSetting.UserId);
                                break;
                            }
                    }
                }
            }

            return result;

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

        private async Task HandleCommunicationCommandAsync(CommunicationCommand communicationCommand)
        {
            switch (communicationCommand.Payload.Body.Channel)
            {
                case Channel.Default:
                    {
                        await SendNotificationMessageToQueueAsync(communicationCommand);
                        return;
                    }
                case Channel.Email:
                    {
                        await SendEmailMessageToQueueAsync(communicationCommand);
                        return;
                    }
                case Channel.SMS:
                    {
                        await SendSmSMessageToQueueAsync(communicationCommand);
                        return;
                    }
                case Channel.System:
                    {
                        await SendSystemMessageMessageToQueueAsync(communicationCommand);
                        return;
                    }
                default:
                    {
                        await Task.CompletedTask;
                        return;
                    }
            }
        }

        private Task SendSystemMessageMessageToQueueAsync(CommunicationCommand communicationCommand)
        {
            throw new NotImplementedException();
        }

        private Task SendSmSMessageToQueueAsync(CommunicationCommand communicationCommand)
        {
            throw new NotImplementedException();
        }

        private async Task SendEmailMessageToQueueAsync(CommunicationCommand communicationCommand)
        {
            var communicationService = _communicationServices.FirstOrDefault(x => x.GetType().Name == typeof(EmailSmtpService).Name);
            var model = new EmailModel
            {
                Body = !string.IsNullOrEmpty(communicationCommand.Payload.Body.Message.DisplayMessage) ? communicationCommand.Payload.Body.Message.DisplayMessage : communicationCommand.Payload.Body.Message.PlainMessage,
                IsHtmlEmail = !string.IsNullOrEmpty(communicationCommand.Payload.Body.Message.DisplayMessage),
                Subject = communicationCommand.Payload.Body.Message.Subject
            };
            if (communicationCommand.Payload.Body.Recipient.Emails != null && communicationCommand.Payload.Body.Recipient.Emails.Any())
            {
                model.Emails = communicationCommand.Payload.Body.Recipient.Emails;


                model.Attachments = communicationCommand.Payload.Body.Message.Attachments;

                if (!communicationCommand.Payload.Body.DirectMessage)
                {
                    var users = await _organizationClientService.GetUsers(_configuration["cxToken"], emails: model.Emails);
                    await CreateNotificationWithHistory(communicationCommand, users.Items.Select(x => x.Identity.ExtId).Distinct().ToHashSet());
                }
                var emailMessage = InitNotificationMessage(communicationCommand, new KeyValuePair<Channel, HashSet<string>>(Channel.Email, new HashSet<string>()), "", null, null, model.Emails);
                await PublishMessageToQueue(emailMessage);
                return;
            }
            if (communicationCommand.Payload.Body.Recipient.UserIds != null)
            {
                int pageIndex = 1;
                var userIds = communicationCommand.Payload.Body.Recipient.UserIds.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToHashSet();
                var hasMore = userIds.Count() > 0;
                while (hasMore)
                {
                    _logger.LogInformation($"Started getting user info for {userIds.Count} user(s)");
                    var users = await _organizationClientService.GetUsers(_configuration["cxToken"], userIds: userIds);
                    if (users.Items.Any())
                    {
                        _logger.LogInformation($"Started sending notification message to queue for {userIds.Count} user(s)");
                        Notification notificationModel = await CreateNotificationWithHistory(communicationCommand, userIds);
                        var keyPairValue = new KeyValuePair<Channel, HashSet<string>>(Channel.Email, userIds);
                        var notificationMessage = InitNotificationMessage(communicationCommand, keyPairValue, notificationModel.Id.ToString(), null, users, null);
                        await PublishMessageToQueue(notificationMessage);

                        _logger.LogInformation($"Finished sending notification for {userIds.Count} user(s). Channel: {Channel.Email}");
                    }
                    else
                    {
                        _logger.LogInformation($"Skipped sending notification. Could not get enough channel info. Channel: {Channel.Email}");
                    }

                    pageIndex++;
                    userIds = communicationCommand.Payload.Body.Recipient.UserIds.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToHashSet();
                    hasMore = userIds.Count() > 0;
                }
                return;
            }
            if (communicationCommand.Payload.Body.Recipient.UserTypeIds != null
                || communicationCommand.Payload.Body.Recipient.DepartmentIds != null
                || communicationCommand.Payload.Body.Recipient.UserGroupIds != null
                || communicationCommand.Payload.Body.Recipient.ForExternalUsers.HasValue
                || communicationCommand.Payload.Body.Recipient.ForHrmsUsers.HasValue)
            {
                var hasMore = true;
                int pageIndex = 1;
                int total = 0;
                Notification notificationModel = await CreateNotification(communicationCommand);
                while (hasMore)
                {
                    var users = await _organizationClientService.GetUsers(cxToken: _configuration["cxToken"],
                        departmentIds: communicationCommand.Payload.Body.Recipient.DepartmentIds, userGroupIds: communicationCommand.Payload.Body.Recipient.UserGroupIds,
                        roles: communicationCommand.Payload.Body.Recipient.UserTypeIds, pageIndex: pageIndex,
                        forExternalUsers: communicationCommand.Payload.Body.Recipient.ForExternalUsers,
                        forHrmsUsers: communicationCommand.Payload.Body.Recipient.ForHrmsUsers);

                    var userIds = users.Items.Select(x => x.Identity.ExtId).Distinct().ToHashSet();
                    await CreateNotificationHistory(notificationModel.Id, userIds, communicationCommand);
                    if (userIds.Any())
                    {
                        try
                        {
                            _logger.LogInformation($"Started sending notification message to queue for {userIds.Count} user(s). Channel: {Channel.Email}");
                            var keyPairValue = new KeyValuePair<Channel, HashSet<string>>(Channel.Email, userIds);
                            var notificationMessage = InitNotificationMessage(communicationCommand, keyPairValue, notificationModel.Id.ToString(), null, users, null);
                            await PublishMessageToQueue(notificationMessage);
                            _logger.LogInformation($"Finished sending notification for {users.Items.Count} user(s). Channel: {Channel.Email}");
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, ex.Message);
                        }
                    }
                    else
                    {
                        _logger.LogInformation($"Skipped sending notification. Could not get enough channel info. Channel: {Channel.Email}");
                    }

                    total += users.Items.Count;
                    pageIndex++;
                    hasMore = users.HasMoreData;
                }
                return;
            }

        }
        private async Task<ISet<string>> GetInstanceIdTokens(ISet<string> userIds)
        {
            var data = await _notificationReferenceRepository.GetAllAsync(x => userIds.Contains(x.UserId) && x.Subscription);
            return data.Select(x => x.InstanceIdToken).ToHashSet();
        }

        private async Task SendNotificationMessageToQueueAsync(CommunicationCommand communicationCommand)
        {
            var communicationService = _communicationServices.FirstOrDefault(x => x.GetType().Name == typeof(FcmPushNotificationService).Name);
            var model = new PushNotificationModel
            {
                Body = communicationCommand.Payload.Body.Message.PlainMessage,
                Subject = communicationCommand.Payload.Body.Message.Subject,
            };
            var notificationMessage = new FireBaseNotificationQueueMessage
            {
                Payload = new Business.Models.Payload(),
                Created = DateTime.UtcNow,
                Routing = new Business.Models.Routing
                {
                    Action = "communication_api.communication.send.notification.requested"
                },
                Type = "command",
                Version = "1.0"
            };
            if (communicationCommand.Payload.Body.Message.IsGlobal.HasValue && communicationCommand.Payload.Body.Message.IsGlobal.Value)
            {
                _logger.LogInformation($"Started sending GLOBAL message...");
                Notification notificationModel = await CreateNotificationWithHistory(communicationCommand, new HashSet<string>());
                _logger.LogInformation($"Finished sending GLOBAL message.");
                return;
            }
            if (communicationCommand.Payload.Body.Recipient.UserIds != null && communicationCommand.Payload.Body.Recipient.UserIds.Any())
            {
                var tokens = await GetInstanceIdTokens(communicationCommand.Payload.Body.Recipient.UserIds);
                _logger.LogInformation($"Started sending notification message to queue for {communicationCommand.Payload.Body.Recipient.UserIds.Count} user(s)");
                Notification notificationModel = await CreateNotificationWithHistory(communicationCommand, communicationCommand.Payload.Body.Recipient.UserIds);

                notificationMessage.Payload.Body = new NotificationMessageBody
                {
                    RegistrationTokens = tokens,
                    DisplayMessage = !string.IsNullOrEmpty(communicationCommand.Payload.Body.Message.DisplayMessage) ? communicationCommand.Payload.Body.Message.DisplayMessage : communicationCommand.Payload.Body.Message.PlainMessage,
                    Subject = communicationCommand.Payload.Body.Message.Subject,
                    Channel = Channel.Default,
                    NotificationId = notificationModel.Id.ToString(),
                    Data = communicationCommand.Payload.Body.Message.Data
                };
                await PublishMessageToQueue(notificationMessage);
                _logger.LogInformation($"Finished sending notification for {communicationCommand.Payload.Body.Recipient.UserIds.Count} user(s)");
                return;
            }
            var userIds = new HashSet<string>();
            if (communicationCommand.Payload.Body.Recipient.UserTypeIds != null || communicationCommand.Payload.Body.Recipient.DepartmentIds != null)
            {
                var hasMore = true;
                int pageIndex = 1;
                int total = 0;
                Notification notificationModel = await CreateNotification(communicationCommand);
                while (hasMore)
                {
                    var users = await _organizationClientService.GetUsers(_configuration["cxToken"],
                        departmentIds: communicationCommand.Payload.Body.Recipient.DepartmentIds,
                        roles: communicationCommand.Payload.Body.Recipient.UserTypeIds, pageIndex: pageIndex);
                    userIds = users.Items.Select(x => x.Identity.ExtId).ToHashSet();
                    if (!userIds.Any())
                    {
                        _logger.LogWarning($"Skipped sending notification, Reason: no user externalid found from {users.Items.Count} user(s)");
                        hasMore = false;
                        break;
                    }
                    model.UserIds = userIds;
                    await CreateNotificationHistory(notificationModel.Id, userIds, communicationCommand);

                    var topic = Guid.NewGuid().ToString();
                    try
                    {
                        _logger.LogInformation($"Started sending notification message to queue for {users.Items.Count} user(s)");
                        var tokens = await GetInstanceIdTokens(userIds);
                        if (tokens.Any())
                        {

                            notificationMessage.Payload.Body = new NotificationMessageBody
                            {
                                RegistrationTokens = tokens,
                                DisplayMessage = !string.IsNullOrEmpty(communicationCommand.Payload.Body.Message.DisplayMessage) ? communicationCommand.Payload.Body.Message.DisplayMessage : communicationCommand.Payload.Body.Message.PlainMessage,
                                Subject = communicationCommand.Payload.Body.Message.Subject,
                                Channel = Channel.Default,
                                NotificationId = notificationModel.Id.ToString(),
                                Data = communicationCommand.Payload.Body.Message.Data,
                            };
                            await PublishMessageToQueue(notificationMessage);
                            _logger.LogInformation($"Finished sending notification for {users.Items.Count} user(s)");
                        }
                        else
                        {
                            _logger.LogInformation($"No token found for {users.Items.Count} user(s)");
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, ex.Message);
                    }
                    total += users.Items.Count;
                    pageIndex++;
                    hasMore = users.HasMoreData;
                }


                return;
            }


        }

        private async Task<Notification> CreateNotificationWithHistory(CommunicationCommand communicationCommand, ISet<string> userIds)
        {
            var notificationModel = new Notification
            {
                Active = true,
                CreatedBy = communicationCommand.Payload.Identity.UserId,
                NotificationType = communicationCommand.Payload.Body.Message.MessageType == null ? NotificationType.Default : (NotificationType)communicationCommand.Payload.Body.Message.MessageType,
                CreatedDateUtc = DateTime.UtcNow,
                ChangedBy = communicationCommand.Payload.Identity.UserId,
                ChangedDateUtc = DateTime.UtcNow,
                TemplateData = communicationCommand.Payload.Body.TemplateData == null ? null : BsonDocument.Parse(JsonConvert.SerializeObject(communicationCommand.Payload.Body.TemplateData)),
                Data = communicationCommand.Payload.Body.Message.Data == null ? null : BsonDocument.Parse(JsonConvert.SerializeObject(communicationCommand.Payload.Body.Message.Data)),
                DefaultBody = GetNotificationBody(communicationCommand.Payload.Body),
                DefaultSubject = communicationCommand.Payload.Body.Message.Subject,
                DefaultPlainTextBody = communicationCommand.Payload.Body.Message.PlainMessage,
                StartDateUtc = communicationCommand.Payload.Body.Message.StartDate,
                EndDateUtc = communicationCommand.Payload.Body.Message.EndDate,
                ExternalId = communicationCommand.Payload.Body.Message.ExternalId,
                ClientId = communicationCommand.Payload.Body.Message.ClientId,
                IsGlobal = communicationCommand.Payload.Body.Message.IsGlobal
            };
            await _notificationRepository.InsertOneAsync(notificationModel);
            var listHistory = new List<NotificationHistory>();
            foreach (var item in userIds)
            {
                listHistory.Add(new NotificationHistory
                {
                    CreatedDateUtc = DateTime.UtcNow,
                    NotificationId = notificationModel.Id,
                    UserId = item,
                    ItemId = communicationCommand.Payload.Body.Message.ExternalId,
                    UserGroupId = communicationCommand.Payload.Body.Recipient.UserGroupIds,
                    ClientId = communicationCommand.Payload.Body.Recipient.ClientIds,
                    UserTypeId = communicationCommand.Payload.Body.Recipient.UserTypeIds,
                    DepartmentId = communicationCommand.Payload.Body.Recipient.DepartmentIds,
                    DepartmentTypeId = communicationCommand.Payload.Body.Recipient.DepartmentTypeIds,
                    Role = communicationCommand.Payload.Body.Recipient.UserTypeIds,
                    NotificationType = communicationCommand.Payload.Body.Message.MessageType == null ? NotificationType.Default : (NotificationType)communicationCommand.Payload.Body.Message.MessageType,
                });
            }
            if (listHistory.Any())
                await _notificationHistoryRepository.InsertManyAsync(listHistory);
            return notificationModel;
        }

        private string GetNotificationBody(Body body)
        {
            if (!string.IsNullOrEmpty(body.Message.PlainMessage))
                return body.Message.PlainMessage;
            if (!string.IsNullOrEmpty(body.Message.DisplayMessage))
                return HtmlExtensions.HtmlToPlainText(body.Message.DisplayMessage, false);
            if (body.TemplateData != null)
            {
                return string.Empty;
            }
            return string.Empty;
        }

        private async Task<Notification> CreateNotification(CommunicationCommand communicationCommand)
        {
            var notificationModel = new Notification
            {
                Active = true,
                CreatedBy = communicationCommand.Payload.Identity.UserId,
                NotificationType = communicationCommand.Payload.Body.Message.MessageType == null ? NotificationType.Default : (NotificationType)communicationCommand.Payload.Body.Message.MessageType,
                CreatedDateUtc = DateTime.UtcNow,
                ChangedBy = communicationCommand.Payload.Identity.UserId,
                ChangedDateUtc = DateTime.UtcNow,
                TemplateData = communicationCommand.Payload.Body.TemplateData == null ? null : BsonDocument.Parse(JsonConvert.SerializeObject(communicationCommand.Payload.Body.TemplateData)),
                Data = communicationCommand.Payload.Body.Message.Data == null ? null : BsonDocument.Parse(JsonConvert.SerializeObject(communicationCommand.Payload.Body.Message.Data)),
                DefaultBody = GetNotificationBody(communicationCommand.Payload.Body),
                DefaultSubject = communicationCommand.Payload.Body.Message.Subject,
                DefaultPlainTextBody = communicationCommand.Payload.Body.Message.PlainMessage,
                StartDateUtc = communicationCommand.Payload.Body.Message.StartDate,
                EndDateUtc = communicationCommand.Payload.Body.Message.EndDate,
                ExternalId = communicationCommand.Payload.Body.Message.ExternalId,
                ClientId = communicationCommand.Payload.Body.Message.ClientId,
                IsGlobal = communicationCommand.Payload.Body.Message.IsGlobal
            };
            await _notificationRepository.InsertOneAsync(notificationModel);
            return notificationModel;
        }

        private async Task CreateNotificationHistory(ObjectId notificationId, ISet<string> userIds, CommunicationCommand communicationCommand)
        {
            var listHistory = new List<NotificationHistory>();
            foreach (var item in userIds)
            {
                listHistory.Add(new NotificationHistory
                {
                    CreatedDateUtc = DateTime.UtcNow,
                    NotificationId = notificationId,
                    UserId = item,
                    ItemId = communicationCommand.Payload.Body.Message.ExternalId,
                    UserGroupId = communicationCommand.Payload.Body.Recipient.UserGroupIds,
                    ClientId = communicationCommand.Payload.Body.Recipient.ClientIds,
                    UserTypeId = communicationCommand.Payload.Body.Recipient.UserTypeIds,
                    DepartmentId = communicationCommand.Payload.Body.Recipient.DepartmentIds,
                    DepartmentTypeId = communicationCommand.Payload.Body.Recipient.DepartmentTypeIds,
                    Role = communicationCommand.Payload.Body.Recipient.UserTypeIds,
                    NotificationType = communicationCommand.Payload.Body.Message.MessageType == null ? NotificationType.Default : (NotificationType)communicationCommand.Payload.Body.Message.MessageType,
                });
            }
            if (listHistory.Any())
                await _notificationHistoryRepository.InsertManyAsync(listHistory);
        }
    }
}
