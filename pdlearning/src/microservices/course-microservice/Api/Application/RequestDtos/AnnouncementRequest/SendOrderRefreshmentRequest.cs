using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microservice.Course.Application.Commands;

namespace Microservice.Course.Application.RequestDtos.AnnouncementRequest
{
    public class SendOrderRefreshmentRequest
    {
        public List<string> SendToEmails { get; set; }

        public List<string> EmailCC { get; set; }

        public string Subject { get; set; }

        public string Base64Message { get; set; }

        public SendOrderRefreshmentCommand ToCommand()
        {
            return new SendOrderRefreshmentCommand()
            {
                SendToEmails = SendToEmails,
                EmailCC = EmailCC,
                Subject = Subject,
                Base64Message = Base64Message
            };
        }
    }
}
