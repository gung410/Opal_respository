using System;
using Conexus.Opal.Connector.RabbitMQ.Contract;
using Microservice.Form.Application.Events.EventPayloads;

namespace Microservice.Form.Application.Events
{
    public class FormNotifySubmittedRequestEvent : BaseRegistrationTodoEvent
    {
        public FormNotifySubmittedRequestEvent(
            FormNotifySubmittedRequestPayload payload,
            ReminderByDto reminderByConditions,
            string subject,
            string template,
            Guid objectId,
            Guid receiverId)
        {
            TaskURI = $"urn:schemas:conexus:dls:content-api:notified-form-submitted:{objectId}";
            Subject = subject;
            Template = template;
            ReceivertId = receiverId;
            Payload = payload;
            ReminderByConditions = reminderByConditions;
        }

        public FormNotifySubmittedRequestPayload Payload { get; }

        public Guid ReceivertId { get; }

        public ReminderByDto ReminderByConditions { get; }
    }
}
