using Microservice.Course.Application.AssociatedEntities;
using Microservice.Course.Domain.Entities;
using Thunder.Platform.Cqrs;

namespace Microservice.Course.Application.Events
{
    public class RegistrationChangeEvent : BaseThunderEvent
    {
        public RegistrationChangeEvent(RegistrationAssociatedEntity registration, RegistrationChangeType changeType, bool isMigrate = false)
        {
            RegistrationAssociatedEntity = registration;
            ChangeType = changeType;
            IsMigrate = isMigrate;
        }

        public RegistrationAssociatedEntity RegistrationAssociatedEntity { get; }

        public RegistrationChangeType ChangeType { get; }

        public bool IsMigrate { get; }

        public override string GetRoutingKey()
        {
            return $"microservice.events.course.registration.{ChangeType.ToString().ToLower()}";
        }
    }
}
