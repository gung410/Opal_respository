using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Text;

namespace Communication.DataAccess.Notification
{
    public class NotificationTemplateLanguage
    {
        public ObjectId Id { get; set; }
        public ObjectId NotificationTemplateId { get; set; }
        public string TemplateContent { get; set; }
        public string TemplateSubject { get; set; }
        public string LanguageCode { get; set; }
    }
}
