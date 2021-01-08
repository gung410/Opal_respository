using System;
using Conexus.Opal.Connector.RabbitMQ.Contract;
using Microservice.Application.Events.EventPayloads;

namespace Microservice.LnaForm.Application.Events
{
    public class FormNotifyTransferOwnershipEvent : BaseRegistrationTodoEvent
    {
        public FormNotifyTransferOwnershipEvent(
            NotifyTransferOwnershipPayload payload,
            ReminderByDto reminderByConditions,
            Guid objectId,
            Guid newOwnerId)
        {
            TaskURI = $"urn:schemas:conexus:dls:content-api:notified-lnaform-transfer-ownership:{objectId}";
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
