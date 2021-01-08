using Microservice.Calendar.Domain.Entities;
using Thunder.Platform.Cqrs;

namespace Microservice.Calendar.Application.Events
{
    public class CommunityEventChangeEvent : BaseThunderEvent
    {
        public CommunityEventChangeEvent(CommunityEvent eventModel, CommunityEventChangeType changeType)
        {
            Event = eventModel;
            ChangeType = changeType;
        }

        public CommunityEvent Event { get; }

        public CommunityEventChangeType ChangeType { get; }

        public override string GetRoutingKey()
        {
            return $"microservice.events.calendar.event.{ChangeType.ToString().ToLower()}";
        }
    }
}
