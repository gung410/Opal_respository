using Communication.Business.MailLog;
using Communication.Business.Models.Email;
using HandlebarsDotNet;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Threading.Tasks;

namespace Communication.Business.Services.Email
{
    public class EmailSmtpService : IEmailSmtpService
    {
        private readonly EmailConfig _configuration;
        private readonly ILogger<EmailSmtpService> _logger;
        private readonly IMailLogger _mailLogger;
        private readonly MailLogConfiguration _mailLogConfig;
        private readonly IWebHostEnvironment _hostingEnvironment;

        public EmailSmtpService(IOptions<EmailConfig> configuration,
            ILogger<EmailSmtpService> logger,
            IWebHostEnvironment hostingEnvironment,
            IMailLogger mailLogger,
            IOptions<MailLogConfiguration> mailLogOption)
        {
            _configuration = configuration.Value;
            _hostingEnvironment = hostingEnvironment;
            _logger = logger;
            _mailLogger = mailLogger;
            _mailLogConfig = mailLogOption.Value;
        }



        public async Task SendEmailsAsync(EmailModel emailModel, string templateContent = null)
        {
            var projectName = emailModel.TemplateData == null ? "Learn" : emailModel.TemplateData.Project;
            _logger.LogDebug($"template content {templateContent}");
            var template = !string.IsNullOrEmpty(templateContent) ? Handlebars.Compile(templateContent) : null;
            using var client = new SmtpClient();
            var credential = new NetworkCredential
            {
                UserName = _configuration.UserName,
                Password = _configuration.UserPassword
            };

            client.Credentials = credential;
            client.Host = _configuration.MailServerAddress;
            client.Port = int.Parse(_configuration.MailServerPort);
            client.EnableSsl = _configuration.SslEnabled;
            foreach (var email in emailModel.Emails)
            {
                try
                {
                    using var emailMessage = new MailMessage();
                    var mailLog = new MailLogMessage()
                    {
                        Recipients = new List<string> { email },
                        SendAtUTC = DateTime.UtcNow,
                        Status = MailStatus.PushToQueue.ToString(),
                        MessageId = Guid.NewGuid().ToString()
                    };
                    var templateData = new Dictionary<string, object>(emailModel.TemplateData.Data);
                    if (emailModel.TemplateData.ReferenceData.ContainsKey(email.Replace(".", "")))
                    {
                        templateData.Add("UserData", emailModel.TemplateData.ReferenceData[email.Replace(".", "")]);
                    }
                    if (emailModel.Attachments != null && emailModel.Attachments.Any())
                    {
                        foreach (var item in emailModel.Attachments)
                        {
                            byte[] bytes = Convert.FromBase64String(item.Content);
                            MemoryStream ms = new MemoryStream(bytes);
                            System.Net.Mail.Attachment attachment = new System.Net.Mail.Attachment(ms, item.Name, item.MediaType);
                            emailMessage.Attachments.Add(attachment);
                        }
                    }
                    string bodyContent = template == null ? emailModel.Body : template(templateData);
                    if (!string.IsNullOrEmpty(bodyContent))
                    {
                        mailLog.HtmlContent = bodyContent;
                    }
                    if (!string.IsNullOrEmpty(emailModel.PlainMessage))
                    {
                        mailLog.PlainTextContent = emailModel.PlainMessage;
                    }
                    mailLog.Subject = emailModel.Subject;

                    emailMessage.Headers.Remove("Message-Id");
                    emailMessage.Headers.Add("Message-Id", mailLog.MessageId);
                    if (_mailLogConfig.Enable && !string.IsNullOrEmpty(_mailLogConfig.ConfigurationSet))
                    {
                        emailMessage.Headers.Add("X-SES-CONFIGURATION-SET", _mailLogConfig.ConfigurationSet);
                    }

                    emailModel.Body = bodyContent;
                    emailMessage.To.Add(new MailAddress(email));
                    emailMessage.From = new MailAddress(_configuration.FromAddress, _configuration.FromName);
                    emailMessage.Subject = emailModel.Subject;
                    emailMessage.Body = emailModel.PlainMessage;
                    var logoPath = Path.Combine(_hostingEnvironment.ContentRootPath, "EmailTemplate", projectName, "Logo", "logo.png");
                    var inlineLogo = new LinkedResource(logoPath, "image/png")
                    {
                        ContentId = "logoid"
                    };
                    ContentType mimeType = new ContentType("text/html");
                    var view = AlternateView.CreateAlternateViewFromString(emailModel.Body, mimeType);
                    view.LinkedResources.Add(inlineLogo);
                    emailMessage.AlternateViews.Add(view);
                    await client.SendMailAsync(emailMessage);
                    foreach (var item in emailMessage.Attachments)
                    {
                        item.Dispose();
                    }
                    _mailLogger.WriteLog(mailLog);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, ex.Message);
                }
            }
        }
    }
}
