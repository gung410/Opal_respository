using System;
using System.Collections.Generic;

namespace Communication.Business.MailLog
{
    public class MailLogMessage
    {
        public string MessageId { get; set; }
        public string HtmlContent { get; set; }
        public string PlainTextContent { get; set; }
        public string Subject { get; set; }
        public string Status { get; set; }
        public DateTime SendAtUTC { get; set; }
        public DateTime? DeliveryAtUTC { get; set; } = null;
        public List<string> Recipients { get; set; }
    }
}
