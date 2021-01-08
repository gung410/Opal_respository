using System.Collections.Generic;

namespace Communication.Business.Models.Email
{
    public class EmailModel : CommunicationModelBase
    {
        public ISet<string> Emails { get; set; }
        public ISet<string> UserIds { get; set; }
        public string PlainMessage { get; set; }
        public bool IsHtmlEmail { get; set; }
        public List<Attachment> Attachments { get; set; }
        public TemplateData TemplateData { get; set; }
    }
}
