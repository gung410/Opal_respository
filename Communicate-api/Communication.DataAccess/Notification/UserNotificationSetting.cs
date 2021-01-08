using System;

using MongoDB.Bson;

namespace Communication.DataAccess.Notification
{
    public class UserNotificationSetting
    {
        public ObjectId Id { get; set; }
        public string UserId { get; set; }
        public NotificationChannel NotificationChannel { get; set; }
        public DigestEmailAt DigestEmailAt { get; set; }
        public bool EnableDigest { get; set; }
    }
    public class DigestEmailAt
    {
        public int Hour { get; set; }
        public int Minute { get; set; }
    }
    public enum NotificationChannel
    {
        Email = 0,
        InAppNotification = 1,
        InAppNotificationAndEmail = 2
    }
}
