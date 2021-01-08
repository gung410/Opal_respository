using Microservice.Course.Domain.Entities;
using Thunder.Platform.Cqrs;

namespace Microservice.Course.Application.Events
{
    public class CourseChangeEvent : BaseThunderEvent
    {
        public CourseChangeEvent(CourseEntity course, CourseChangeType changeType, bool isMigrate = false)
        {
            Course = course;
            ChangeType = changeType;
            IsMigrate = isMigrate;
        }

        public CourseEntity Course { get; }

        public CourseChangeType ChangeType { get; }

        public bool IsMigrate { get; }

        public override string GetRoutingKey()
        {
            return $"microservice.events.course.{ChangeType.ToString().ToLower()}";
        }
    }
}
