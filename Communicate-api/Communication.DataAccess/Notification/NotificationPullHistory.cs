using System;

using MongoDB.Bson.Serialization.Attributes;

namespace Communication.DataAccess.Notification
{
    public class NotificationPullHistory
    {
        [BsonId]
        public string UserId { get; set; }
        public DateTime PullingAtUtc { get; set; }
    }
}
