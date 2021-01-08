using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

namespace Communication.DataAccess.Notification
{
    [BsonIgnoreExtraElements]
    public class Notification
    {
        public ObjectId Id { get; set; }
        public NotificationType?  NotificationType { get; set; }
        public NotificationPurpose NotificationPurpose { get; set; }
        public bool Active { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDateUtc { get; set; }
        public string ChangedBy { get; set; }
        public string DefaultBody { get; set; }
        public string DefaultSubject { get; set; }
        public string DefaultPlainTextBody { get; set; }
        public DateTime ChangedDateUtc { get; set; }
        public DateTime? StartDateUtc { get; set; }
        public DateTime? EndDateUtc { get; set; }
        public DateTime? SentDateUtc { get; set; }
        public ObjectId? NotificationTemplateId { get; set; }
        public string ClientId { get; set; }
        public string ExternalId { get; set; }
        public bool? IsGlobal { get; set; }
        public BsonDocument Data { get; set; }
        public BsonDocument TemplateData { get; set; }
    }
}