using Microservice.Course.Application.AssociatedEntities;
using Thunder.Platform.Cqrs;

namespace Microservice.Course.Application.Events
{
    public class ClassRunChangeEvent : BaseThunderEvent
    {
        public ClassRunChangeEvent(ClassRunAssociatedEntity classRun, ClassRunChangeType changeType, bool isMigrate = false)
        {
            ClassRun = classRun;
            ChangeType = changeType;
            IsMigrate = isMigrate;
        }

        public ClassRunAssociatedEntity ClassRun { get; }

        public ClassRunChangeType ChangeType { get; }

        public bool IsMigrate { get; }

        public override string GetRoutingKey()
        {
            return $"microservice.events.course.classrun.{ChangeType.ToString().ToLower()}";
        }
    }
}
