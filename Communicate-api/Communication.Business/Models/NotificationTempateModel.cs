using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Text;

namespace Communication.Business.Models
{
    public class NotificationTempateModel
    {
        public string Id { get; set; }
        public string Tag { get; set; }
        public string Name { get; set; }
        public DateTime CreatedDateUtc { get; set; }
        public DateTime ModifiedDateUtc { get; set; }
        public string Version { get; set; }
        public List<NotificationTemplateLanguageModel> TemplateLanguages { get; set; }
    }
}
