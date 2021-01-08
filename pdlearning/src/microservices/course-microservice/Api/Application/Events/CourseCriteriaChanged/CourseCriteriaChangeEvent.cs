using Microservice.Course.Domain.Entities;
using Thunder.Platform.Cqrs;

namespace Microservice.Course.Application.Events
{
    public class CourseCriteriaChangeEvent : BaseThunderEvent
    {
        public CourseCriteriaChangeEvent(CourseCriteria courseCriteria, CourseCriteriaChangeType changeType)
        {
            CourseCriteria = courseCriteria;
            ChangeType = changeType;
        }

        public CourseCriteria CourseCriteria { get; }

        public CourseCriteriaChangeType ChangeType { get; }

        public override string GetRoutingKey()
        {
            return $"microservice.events.course.coursecriteria.{ChangeType.ToString().ToLower()}";
        }
    }
}
