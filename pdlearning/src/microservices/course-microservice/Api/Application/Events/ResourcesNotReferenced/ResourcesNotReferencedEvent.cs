using System;
using System.Collections.Generic;
using Thunder.Platform.Cqrs;

namespace Microservice.Course.Application.Events
{
#pragma warning disable SA1402 // File may only contain a single type
    public class ResourcesNotReferencedEvent : BaseThunderEvent
    {
        public ResourcesNotReferencedEvent(List<Guid> objectIds, ResourcesNotReferencedType resourcesNotReferencedType)
        {
            Body = new ResourcesNotReferencedEventBody(objectIds);
            RoutingKey = $"microservice.events.{resourcesNotReferencedType}.courses-not-referenced".ToLowerInvariant();
        }

        public ResourcesNotReferencedEventBody Body { get; }

        private string RoutingKey { get; }

        public override string GetRoutingKey()
        {
            return RoutingKey;
        }
    }

    public class ResourcesNotReferencedEventBody
    {
        public ResourcesNotReferencedEventBody(List<Guid> objectIds)
        {
            ObjectIds = objectIds;
        }

        public List<Guid> ObjectIds { get; }
    }
#pragma warning restore SA1402 // File may only contain a single type
}
