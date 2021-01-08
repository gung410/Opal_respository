using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Communication.Business.Models
{
    public class NotificationDigestModel
    {
        public string UserId { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public string SentDateUtc { get; set; }
    }
}
