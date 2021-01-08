using System;
using Conexus.Opal.Connector.RabbitMQ.Contract;
using Microservice.StandaloneSurvey.Application.Events.EventPayloads;

namespace Microservice.StandaloneSurvey.Application.Events
{
    public enum FormParticipantNotifyType
    {
        Assign,
        Remind,
        Remove
    }

    public class FormParticipantNotifyEvent : BaseRegistrationTodoEvent
    {
        public FormParticipantNotifyEvent(
            NotifyFormParticipantPayload payload,
            ReminderByDto reminderByConditions,
            Guid participantId,
            Guid objectId,
            string template,
            string subject)
        {
            TaskURI = $"urn:schemas:conexus:dls:content-api:notified-form-participant:{objectId}";
            Subject = subject;
            Template = template;
            Payload = payload;
            ParticipantId = participantId;
            ReminderByConditions = reminderByConditions;
        }

        public NotifyFormParticipantPayload Payload { get; }

        public Guid ParticipantId { get; }

        public ReminderByDto ReminderByConditions { get; }
    }
}
