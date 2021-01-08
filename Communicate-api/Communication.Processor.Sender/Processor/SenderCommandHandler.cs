using Communication.Business.HttpClients;
using Communication.Business.Models;
using Communication.Business.Models.Email;
using Communication.Business.Services;
using Communication.Business.Services.Email;
using Communication.Business.Services.FirebaseCloudMessage;
using Datahub.Processor.Base.ProcessorRegister;
using Datahub.Processor.Base.Settings;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Serialization;
using Communication.Business.Models.Command;
using Communication.Business.Models.FirebaseCloudMessage;
using System.IO;
using HandlebarsDotNet;

namespace Communication.Processor.Sender.Processor
{
    public class SenderCommandHandler : ActionHandlerBase, IActionHandler
    {
        private IEnumerable<ICommunicationService> _communicationServices;
        private IIdentityServerClientService _identityServerClientService;
        private IConfiguration _configuration;
        private IWebHostEnvironment _hostingEnvironment;
        private IConnection _connection;
        private ConnectionFactory _factory;
        private readonly IServiceProvider _serviceProvider;
        public SenderCommandHandler(ILogger<SenderCommandHandler> logger, IServiceProvider serviceProvider) : base(logger)
        {
            _serviceProvider = serviceProvider;
        }

        public override string Action => ActionConstants.AcceptAll;

        public override Func<dynamic, Task> Handler => async dynamicObject =>
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                _communicationServices = scope.ServiceProvider.GetServices<ICommunicationService>();
                _identityServerClientService = scope.ServiceProvider.GetService<IIdentityServerClientService>();
                _configuration = scope.ServiceProvider.GetService<IConfiguration>();
                _hostingEnvironment = scope.ServiceProvider.GetService<IWebHostEnvironment>();
                _connection = scope.ServiceProvider.GetService<IConnection>();
                _factory = scope.ServiceProvider.GetService<ConnectionFactory>();
            }
            if (dynamicObject.Type != "command")
                return;
            var message = DeserializeObject<QueueMessageBase>(dynamicObject);
            await HandleCommunicationCommandAsync(message);
        };

        private T DeserializeObject<T>(dynamic dynamicObject) where T : class
        {
            var messageStr = JsonConvert.SerializeObject(dynamicObject, new JsonSerializerSettings
            {
                ContractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new CustomNamingStrategy()
                }
            });
            return  JsonConvert.DeserializeObject<T>(messageStr);
        }

        private async Task HandleCommunicationCommandAsync(QueueMessageBase communicationCommand)
        {
            var body = DeserializeObject<MessageBody>(communicationCommand.Payload.Body);
            switch (body.Channel)
            {
                case Channel.Default:
                    {
                        await SendNotificationMessageAsync(communicationCommand);
                        return;
                    }
                case Channel.Email:
                    {
                        await SendEmailMessageAsync(communicationCommand);
                        return;
                    }
                case Channel.SMS:
                    {
                        await SendSmSMessageAsync(communicationCommand);
                        return;
                    }
                case Channel.System:
                    {
                        await SendNotificationMessageAsync(communicationCommand);
                        return;
                    }
                default:
                    {
                        await Task.CompletedTask;
                        return;
                    }
            }
        }

        private Task SendSystemMessageMessageToQueueAsync(QueueMessageBase communicationCommand)
        {
            throw new NotImplementedException();
        }

        private Task SendSmSMessageAsync(QueueMessageBase communicationCommand)
        {
            throw new NotImplementedException();
        }

        private async Task SendEmailMessageAsync(QueueMessageBase communicationCommand)
        {
            var communicationService = _communicationServices.FirstOrDefault(x => x.GetType().Name == typeof(EmailService).Name);
            var messageBody = DeserializeObject<EmailMessageBody>(communicationCommand.Payload.Body);
            var model = GenerateEmailModelFromEmailBodyMessage(messageBody);

            _logger.LogInformation($"Started sending email for {model.Emails.Count} email(s)");
            await communicationService.SendCommunicationAsync(model);
            _logger.LogInformation($"Finished sending email for {model.Emails.Count} email(s)");
        }

        private EmailModel GenerateEmailModelFromEmailBodyMessage(EmailMessageBody emailMessageBody)
        {
            var model = new EmailModel();
            string displayMessage = emailMessageBody.DisplayMessage;
            string plainMessage = emailMessageBody.PlainMessage;
            bool isHtml = emailMessageBody.IsHtmlEmail;
            model.Body = !string.IsNullOrEmpty(displayMessage) ? displayMessage : plainMessage;
            model.IsHtmlEmail = isHtml;
            model.PlainMessage = plainMessage;
            model.Emails = emailMessageBody.Emails?.ToHashSet();
            model.Subject = emailMessageBody.Subject;
            model.Attachments = emailMessageBody.Attachments;
            if (emailMessageBody.TemplateData != null)
            {
                model.TemplateData = emailMessageBody.TemplateData;
            }
            else
            {
                if (_configuration["PROJECT_NAME"].ToLower() == "opal")
                {
                    //Todo: for testing right now
                    model.TemplateData = new TemplateData
                    {
                        Project = "Opal",
                        Module = "SystemAdmin",
                        TemplateName = "SystemNotification",
                        Data = new Dictionary<string, object>
                        {
                            {"Body", emailMessageBody.DisplayMessage},
                            {"Title", emailMessageBody.Subject},
                            {"Subject", emailMessageBody.Subject}
                        }
                    };
                }
            }

            return model;
        }




        private async Task SendNotificationMessageAsync(QueueMessageBase communicationCommand)
        {
            var body = DeserializeObject<NotificationMessageBody>(communicationCommand.Payload.Body);
            var communicationService = _communicationServices.FirstOrDefault(x => x.GetType().Name == typeof(FcmPushNotificationService).Name);
            var model = new PushNotificationModel
            {
                //Body = communicationCommand.Payload.Body.DisplayMessage,
                Subject = body.Subject,
                Data = body.Data,
                TemplateData = body.TemplateData
            };
            string textSource;

            if (model.TemplateData != null)
            {
                if (string.IsNullOrEmpty(body.DisplayMessage))
                {
                    var file = Path.Combine(_hostingEnvironment.ContentRootPath, "NotificationTemplate", model.TemplateData.Project,
                        model.TemplateData.Module, model.TemplateData.TemplateName + ".txt");
                    textSource = File.ReadAllText(file);
                    var template = !string.IsNullOrEmpty(textSource) ? Handlebars.Compile(textSource) : null;
                    textSource = template == null ? textSource : template(model.TemplateData.Data);
                }
                else
                {
                    textSource = body.DisplayMessage;
                }
            }
            else
            {
                textSource = body.DisplayMessage;
            }

            if (model.Data != null)
                model.Data.NotificationId = body.NotificationId;
            model.Body = textSource;
            var data = body.RegistrationTokens;
            _logger.LogInformation($"Started sending notification for {data.Count} user(s)");
            var topic = Guid.NewGuid().ToString();
            await communicationService.SubscribeAsync(new HashSet<string> { topic }, data);
            model.TopicNames = new HashSet<string> { topic };
            await communicationService.SendCommunicationAsync(model);
            await communicationService.UnSubscribeAsync(new HashSet<string> { topic }, data);
            _logger.LogInformation($"Finished sending notification for {data.Count} user(s)");
            return;
        }


    }
}
