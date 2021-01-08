using Microservice.Course.Domain.Entities;
using Thunder.Platform.Cqrs;

namespace Microservice.Course.Application.Events
{
    public class SectionChangeEvent : BaseThunderEvent
    {
        public SectionChangeEvent(Section section, SectionChangeType changeType, bool isMigrate = false)
        {
            Section = section;
            ChangeType = changeType;
            IsMigrate = isMigrate;
        }

        public Section Section { get; }

        public SectionChangeType ChangeType { get; }

        public bool IsMigrate { get; }

        public override string GetRoutingKey()
        {
            return $"microservice.events.course.section.{ChangeType.ToString().ToLower()}";
        }
    }
}
