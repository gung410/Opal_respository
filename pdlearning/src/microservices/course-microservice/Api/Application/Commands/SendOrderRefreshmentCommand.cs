using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Thunder.Platform.Cqrs;

namespace Microservice.Course.Application.Commands
{
    public class SendOrderRefreshmentCommand : BaseThunderCommand
    {
        public List<string> SendToEmails { get; set; }

        public List<string> EmailCC { get; set; }

        public string Subject { get; set; }

        public string Base64Message { get; set; }

        public string GetDecodedMessage()
        {
            return Base64Message == null ? string.Empty : Encoding.UTF8.GetString(Convert.FromBase64String(Base64Message));
        }
    }
}
