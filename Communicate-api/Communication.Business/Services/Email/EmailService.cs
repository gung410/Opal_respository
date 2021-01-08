using Communication.Business.Models;
using Communication.Business.Models.Email;
using Communication.Business.Services;
using Communication.Business.Services.FirebaseCloudMessage;
using Communication.DataAccess.Notification;
using HandlebarsDotNet;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Communication.Business.Services.Email
{
    public class EmailService : BaseCommnunicationService
    {
        private readonly IEmailSmtpService _emailService;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly ILogger<EmailService> _logger;
        private readonly IUserNotificationSettingRepository _userNotificationSettingRepository;
        public EmailService(IEmailSmtpService emailService, IWebHostEnvironment hostingEnvironment, INotificationHistoryRepository notificationHistoryRepository,
            INotificationRepository notificationRepository, INotificationPullHistoryRepository notificationPullHistoryRepository, ILogger<EmailService> logger, IUserNotificationSettingRepository userNotificationSettingRepository) :
            base(notificationHistoryRepository, notificationRepository, notificationPullHistoryRepository, hostingEnvironment, logger, userNotificationSettingRepository)
        {
            _emailService = emailService;
            _hostingEnvironment = hostingEnvironment;
            _logger = logger;
            _userNotificationSettingRepository = userNotificationSettingRepository;
        }


        public override Task SendCommunicationAsync(CommunicationModelBase emailModel)
        {
            var model = emailModel as EmailModel;

            if (model.TemplateData != null)
            {
                string source;
                if (string.IsNullOrEmpty(model.Body))
                {
                    var file = Path.Combine(_hostingEnvironment.ContentRootPath, "EmailTemplate", model.TemplateData.Project,
                        model.TemplateData.Module, model.TemplateData.TemplateName + ".html");
                    source = File.ReadAllText(file);
                }
                else
                {
                    source = model.Body;
                }
                return _emailService.SendEmailsAsync(model, source);
            }
            return _emailService.SendEmailsAsync(model);

        }

    }
}
