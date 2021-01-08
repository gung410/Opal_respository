using System;
using Conexus.Opal.Connector.RabbitMQ.Contract;

namespace Microservice.BrokenLink.Application.Events
{
    public class NotifyBrokenLinkEvent : BaseTodoRegistrationEvent
    {
        public NotifyBrokenLinkEvent(
            NotifyBrokenLinkPayload payload,
            Guid ownerId,
            Guid objectId,
            ReminderByDto reminderByConditions,
            string template)
        {
            TaskURI = $"urn:schemas:conexus:dls:content-api:notified-broken-link:{objectId}";
            Subject = $"OPAL2.0 - Broken link is detected in {payload.AssetName}";
            Template = template;
            Payload = payload;
            OwnerId = ownerId;
            ReminderByConditions = reminderByConditions;
        }

        public NotifyBrokenLinkPayload Payload { get; }

        public Guid OwnerId { get; }

        public ReminderByDto ReminderByConditions { get; }
    }
}
