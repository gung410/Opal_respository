using Microservice.Course.Application.Models;
using Thunder.Platform.Cqrs;

namespace Microservice.Course.Application.Events
{
    public class AssignmentChangeEvent : BaseThunderEvent
    {
        public AssignmentChangeEvent(AssignmentModel assignment, AssignmentChangeType changeType, bool isMigrate = false)
        {
            Assignment = assignment;
            ChangeType = changeType;
            IsMigrate = isMigrate;
        }

        public AssignmentModel Assignment { get; }

        public AssignmentChangeType ChangeType { get; }

        public bool IsMigrate { get; }

        public override string GetRoutingKey()
        {
            return $"microservice.events.course.assignment.{ChangeType.ToString().ToLower()}";
        }
    }
}
