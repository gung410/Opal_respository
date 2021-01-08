using MongoDB.Bson;
using System;
using System.Collections.Generic;

namespace Communication.DataAccess.Notification
{
    public class NotificationSubcription
    {
        public ObjectId Id { get; set; }
        public string TopicName { get; set; }
        public string UserId { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool IsActive { get; set; }
        public DateTime LastModifiedDate { get; set; }
    }
}