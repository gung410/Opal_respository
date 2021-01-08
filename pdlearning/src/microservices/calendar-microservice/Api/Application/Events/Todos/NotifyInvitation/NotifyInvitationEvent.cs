using System;
using Conexus.Opal.Connector.RabbitMQ.Contract;

namespace Microservice.Calendar.Application.Events
{
    public class NotifyInvitationEvent : BaseTodoRegistrationEvent
    {
        public NotifyInvitationEvent(
            NotifyInvitationPayload payload,
            Guid eventId,
            Guid createdBy,
            Guid attendeeId,
            ReminderByDto reminderByConditions,
            string templateName = "ContentNotifyNewEventInvitation")
        {
            TaskURI = $"urn:schemas:conexus:dls:calendar-api:notified-invitation:{eventId}";
            Subject = $"OPAL2.0 - New event invited - {payload.SubjectContent}.";
            Payload = payload;
            AttendeeId = attendeeId;
            ReminderByConditions = reminderByConditions;
            CreatedBy = createdBy;
            Template = templateName;
        }

        public Guid AttendeeId { get; }

        public NotifyInvitationPayload Payload { get; }

        public ReminderByDto ReminderByConditions { get; }
    }
}
