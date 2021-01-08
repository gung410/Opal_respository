using System;
using Conexus.Opal.Connector.RabbitMQ.Contract;

namespace Microservice.Content.Application.Events
{
    public class NotifyContentSubmittedRequestEvent : BaseContentTodoRegistrationEvent
    {
        public NotifyContentSubmittedRequestEvent(
            NotifyContentSubmittedRequestPayload payload,
            ReminderByDto reminderByConditions,
            string subject,
            string template,
            Guid objectId,
            Guid receiverId)
        {
            TaskURI = $"urn:schemas:conexus:dls:content-api:notified-content-submitted:{objectId}";
            Subject = subject;
            Template = template;
            ReceiverId = receiverId;
            Payload = payload;
            ReminderByConditions = reminderByConditions;
        }

        public NotifyContentSubmittedRequestPayload Payload { get; }

        public Guid ReceiverId { get; }

        public ReminderByDto ReminderByConditions { get; }
    }
}
