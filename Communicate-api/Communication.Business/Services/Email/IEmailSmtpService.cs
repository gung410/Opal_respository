using Communication.Business.Models.Email;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Communication.Business.Services.Email
{
    public interface IEmailSmtpService
    {
        Task SendEmailsAsync(EmailModel emailModel, string templateContent = null);
    }
}
