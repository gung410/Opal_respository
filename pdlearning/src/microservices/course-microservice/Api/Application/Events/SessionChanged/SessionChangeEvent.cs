using Microservice.Course.Application.AssociatedEntities;
using Thunder.Platform.Cqrs;

namespace Microservice.Course.Application.Events
{
    public class SessionChangeEvent : BaseThunderEvent
    {
        public SessionChangeEvent(SessionAssociatedEntity session, SessionChangeType changeType)
        {
            Session = session;
            ChangeType = changeType;
        }

        public SessionAssociatedEntity Session { get; }

        public SessionChangeType ChangeType { get; }

        public override string GetRoutingKey()
        {
            return $"microservice.events.course.session.{ChangeType.ToString().ToLower()}";
        }
    }
}
