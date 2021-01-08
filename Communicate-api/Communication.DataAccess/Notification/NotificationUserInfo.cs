using MongoDB.Bson;
using System;

namespace Communication.DataAccess.Notification
{
    public class NotificationUserInfo
    {
        public ObjectId Id { get; set; }
        public string UserId { get; set; }
        public string DeviceId { get; set; }
        public string InstanceIdToken { get; set; }
        public string Platform { get; set; }
        public string ClientId { get; set; }
        public DateTime CreatedDateUtc { get; set; }
        public DateTime ModifiedDateUtc { get; set; }
        public bool Subscription { get; set; }
    }
}