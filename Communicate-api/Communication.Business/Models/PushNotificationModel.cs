using Communication.Business.Models.Email;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Communication.Business.Models
{
    public class PushNotificationModel : CommunicationModelBase
    {
        [Required]
        public ISet<string> TopicNames { get; set; }
        public dynamic Data {get;set;}
        [Required]
        public ISet<string> UserIds { get; set; }
        public TemplateData TemplateData { get; set; }
    }
}
