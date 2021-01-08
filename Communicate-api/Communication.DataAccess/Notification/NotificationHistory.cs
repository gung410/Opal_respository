using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

namespace Communication.DataAccess.Notification
{
    [BsonIgnoreExtraElements]
    public class NotificationHistory
    {
        public ObjectId Id { get; set; }
        public ObjectId NotificationId { get; set; }
        public DateTime CreatedDateUtc { get; set; }
        public DateTime? DateReadUtc { get; set; }
        public bool Cancelled { get; set; }
        public DateTime? CancelledDateUtc { get; set; }
        public DateTime? DeletedDateUtc { get; set; }
        public string UserId { get; set; }
        public HashSet<string> DepartmentId { get; set; }
        public HashSet<string> UserGroupId { get; set; }
        public HashSet<string> DepartmentTypeId { get; set; }
        public HashSet<string> UserTypeId { get; set; }
        public HashSet<string> ClientId { get; set; }
        public HashSet<string> Role { get; set; }
        public string ItemId { get; set; }
        public NotificationType? NotificationType { get; set; }


    }
}