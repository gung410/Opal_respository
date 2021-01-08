using Microservice.Learner.Domain.Entities;
using Thunder.Platform.Cqrs;

namespace Microservice.Learner.Application.Events
{
    public enum AssignmentChangeType
    {
        Created,
        Updated
    }

    public class AssignmentChangeEvent : BaseThunderEvent
    {
        public AssignmentChangeEvent(MyAssignment assignment, AssignmentChangeType changeType)
        {
            Assignment = assignment;
            ChangeType = changeType;
        }

        public MyAssignment Assignment { get; set; }

        public AssignmentChangeType ChangeType { get; set; }

        public override string GetRoutingKey()
        {
            return $"microservice.events.learner.assignment.{ChangeType.ToString().ToLower()}";
        }
    }
}
