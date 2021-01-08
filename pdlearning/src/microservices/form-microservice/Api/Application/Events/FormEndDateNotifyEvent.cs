using System;
using Conexus.Opal.Connector.RabbitMQ.Contract;
using Microservice.Form.Application.Events.EventPayloads;

namespace Microservice.Form.Application.Events
{
    public class FormEndDateNotifyEvent : BaseRegistrationTodoEvent
    {
        public FormEndDateNotifyEvent(
            NotifyFormDueDatePayload payload,
            ReminderByDto reminderByConditions,
            Guid objectId,
            Guid receiverId)
        {
            TaskURI = $"urn:schemas:conexus:dls:content-api:notified-form-end-date:{objectId}";
            Subject = $"Due date for survey {payload.FormName} is in {payload.RemindBeforeDays} day(s)";
            Template = "SurveyNotifyEndDate";
            ReceivertId = receiverId;
            Payload = payload;
            ReminderByConditions = reminderByConditions;
        }

        public NotifyFormDueDatePayload Payload { get; }

        public Guid ReceivertId { get; }

        public ReminderByDto ReminderByConditions { get; }
    }
}
