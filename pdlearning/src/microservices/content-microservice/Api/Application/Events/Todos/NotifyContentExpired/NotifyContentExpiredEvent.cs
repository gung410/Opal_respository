using System;
using Conexus.Opal.Connector.RabbitMQ.Contract;

namespace Microservice.Content.Application.Events
{
    public class NotifyContentExpiredEvent : BaseContentTodoRegistrationEvent
    {
        public NotifyContentExpiredEvent(
            NotifyContentExpiredPayload payload,
            Guid contentCreatorId,
            Guid digitalContentId,
            DateTime expiredDateTime,
            ReminderByDto reminderByConditions)
        {
            TaskURI = $"urn:schemas:conexus:dls:content-api:notified-content-expired:{digitalContentId}";
            Subject = "OPAL2.0 - Your Content is expired";
            Template = "ContentExpiredSystemAlert";
            Payload = payload;
            ContentCreatorId = contentCreatorId;
            ReminderByConditions = reminderByConditions;
            ExpiredDateTime = expiredDateTime;
        }

        public NotifyContentExpiredPayload Payload { get; }

        public Guid ContentCreatorId { get; }

        public ReminderByDto ReminderByConditions { get; }

        public DateTime ExpiredDateTime { get; }
    }
}
