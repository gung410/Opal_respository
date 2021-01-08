using System;

using Communication.DataAccess.Notification;

namespace Communication.Business.Models.UserNotificationSetting
{
    public class UserNotificationSettingCommand
    {
        public string UserId { get; set; }
        public NotificationChannel NotificationChannel { get; set; }
        public bool EnableDigest { get; set; }
        public DigestEmailAt DigestEmailAt { get; set; }
    }
}
