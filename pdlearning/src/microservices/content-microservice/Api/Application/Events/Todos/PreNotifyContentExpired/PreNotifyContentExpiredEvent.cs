using System;
using Conexus.Opal.Connector.RabbitMQ.Contract;

namespace Microservice.Content.Application.Events
{
    public class PreNotifyContentExpiredEvent : BaseContentTodoRegistrationEvent
    {
        public PreNotifyContentExpiredEvent(
            PreNotifyContentExpiredPayload payload,
            Guid contentCreatorId,
            Guid digitalContentId,
            ReminderByDto reminderByConditions)
        {
            TaskURI = $"urn:schemas:conexus:dls:content-api:pre-notified-content-expired:{digitalContentId}";
            Subject = $"OPAL2.0 - {payload.Subject}";
            Template = payload.Template;
            Payload = payload;
            ContentCreatorId = contentCreatorId;
            ReminderByConditions = reminderByConditions;
        }

        public PreNotifyContentExpiredPayload Payload { get; }

        public Guid ContentCreatorId { get; }

        public ReminderByDto ReminderByConditions { get; }
    }
}
