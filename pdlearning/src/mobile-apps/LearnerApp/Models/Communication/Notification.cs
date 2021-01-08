using System;

namespace LearnerApp.Models.Communication
{
    public class Notification
    {
        public string MessageId { get; set; }

        public string NotificationId { get; set; }

        public string Subject { get; set; }

        public string NotificationType { get; set; }

        public string Body { get; set; }

        public bool IsGlobalMessage { get; set; }

        public bool New { get; set; }

        public DateTime StartDateUtc { get; set; }

        public DateTime EndDateUtc { get; set; }

        public DateTime CreatedDateUtc { get; set; }

        public DateTime DateReadUtc { get; set; }
    }
}
