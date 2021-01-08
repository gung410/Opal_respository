using Microservice.Course.Domain.Entities;
using Thunder.Platform.Cqrs;

namespace Microservice.Course.Application.Events
{
    public class CoursePlanningCycleChangeEvent : BaseThunderEvent
    {
        public CoursePlanningCycleChangeEvent(CoursePlanningCycle coursePlanningCycle, CoursePlanningCycleChangeType changeType)
        {
            CoursePlanningCycle = coursePlanningCycle;
            ChangeType = changeType;
        }

        public CoursePlanningCycle CoursePlanningCycle { get; }

        public CoursePlanningCycleChangeType ChangeType { get; }

        public override string GetRoutingKey()
        {
            return $"microservice.events.course.courseplanningcycle.{ChangeType.ToString().ToLower()}";
        }
    }
}
