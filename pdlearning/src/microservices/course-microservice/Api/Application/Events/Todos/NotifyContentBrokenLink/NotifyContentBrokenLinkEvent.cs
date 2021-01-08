using System;
using System.Collections.Generic;
using Conexus.Opal.Connector.RabbitMQ.Contract;

namespace Microservice.Course.Application.Events.Todos
{
    public class NotifyContentBrokenLinkEvent : BaseTodoEvent<NotifyContentBrokenLinkPayload>
    {
        public NotifyContentBrokenLinkEvent(
            NotifyContentBrokenLinkPayload payload,
            List<Guid> assignedToIds,
            string template,
            ReminderByDto reminderByConditions)
        {
            TaskURI = $"urn:schemas:conexus:dls:content-api:notified-content-broken-link:{Guid.NewGuid()}";
            Subject = $"Broken link is detected in {payload.AssetName}";
            Template = template;
            Payload = payload;
            AssignedToIds = assignedToIds;
            ReminderByConditions = reminderByConditions;
        }

        public List<Guid> AssignedToIds { get; }

        public ReminderByDto ReminderByConditions { get; }
    }
}
