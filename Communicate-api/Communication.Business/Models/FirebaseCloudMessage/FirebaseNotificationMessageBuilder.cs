namespace Communication.Business.Models.FirebaseCloudMessage
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class FirebaseNotificationMessageBuilder
    {
        private FirebaseNotificationMessage notificationMessage;
        public FirebaseNotificationMessageBuilder()
        {
            notificationMessage = new FirebaseNotificationMessage();
            notificationMessage.Priority = "high";
            notificationMessage.ContentAvailable = true;
            //notificationMessage.Notification = new NotificationPayload();
            //notificationMessage.Notification.Sound = "default";
        }
        public static FirebaseNotificationMessageBuilder GetNotificationMessageBuilder()
        {
            return new FirebaseNotificationMessageBuilder();
        }
        public FirebaseNotificationMessageBuilder WithTitleAndBody(string title, string body)
        {
            notificationMessage.Notification = new NotificationPayload();
            notificationMessage.Notification.Title = title;
            notificationMessage.Notification.Body = body;
            return this;
        }
        public FirebaseNotificationMessageBuilder WithData(dynamic data)
        {
            notificationMessage.Data = data;
            return this;
        }
        public FirebaseNotificationMessageBuilder ToRegistrationToken(string token)
        {
            notificationMessage.To = token;
            return this;
        }
        public FirebaseNotificationMessageBuilder ToTopic(string topic)
        {
            notificationMessage.To = string.Format("/topics/{0}", topic);
            return this;
        }
        public FirebaseNotificationMessageBuilder ToMultipleTopic(ISet<string> topics)
        {
            string topicConditionFormat = "'{0}' in topics";

            ISet<string> result = topics.Select(t => string.Format(topicConditionFormat, t)).ToHashSet<string>();

            notificationMessage.Condition = string.Join(" || ", result.ToArray());

            return this;
        }
        public FirebaseNotificationMessage Build()
        {
            return notificationMessage;
        }
    }
}