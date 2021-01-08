using System;
using Conexus.Opal.Connector.RabbitMQ.Contract;
using Microservice.Application.Events.EventPayloads;
using Microservice.Content.Application.Events;

namespace Microservice.Form.Application.Events
{
    public class NotifyContentTransferOwnershipEvent : BaseContentTodoRegistrationEvent
    {
        public NotifyContentTransferOwnershipEvent(
            NotifyTransferOwnershipPayload payload,
            ReminderByDto reminderByConditions,
            Guid objectId,
            Guid newOwnerId)
        {
            TaskURI = $"urn:schemas:conexus:dls:content-api:notified-content-transfer-ownership:{objectId}";
            Subject = $"OPAL2.0 - Ownership transfer for {payload.ObjectName}";
            Template = "NotifyTransferOwnershipTemplate";
            NewOwnerId = newOwnerId;
            Payload = payload;
            ReminderByConditions = reminderByConditions;
        }

        public NotifyTransferOwnershipPayload Payload { get; }

        public Guid NewOwnerId { get; }

        public ReminderByDto ReminderByConditions { get; }
    }
}
