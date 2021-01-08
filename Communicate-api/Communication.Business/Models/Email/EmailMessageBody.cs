using Communication.Business.Models;
using Communication.Business.Models.Email;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Communication.Business.Models.Email
{
    public class EmailMessageBody : MessageBody
    {
        public string PlainMessage { get; set; }
        public bool IsHtmlEmail { get; set; }
        public ISet<string> Emails { get; set; }
        public List<Attachment> Attachments { get; set; }
    }

}