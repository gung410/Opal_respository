using Microservice.Learner.Domain.Entities;
using Microservice.Learner.Domain.ValueObject;
using Thunder.Platform.Cqrs;

namespace Microservice.Learner.Application.Events
{
    public class MyCourseChangeEvent : BaseThunderEvent
    {
        public MyCourseChangeEvent(MyCourse myCourse, MyCourseStatus changeType)
        {
            MyCourse = myCourse;
            ChangeType = changeType;
        }

        public MyCourse MyCourse { get; set; }

        private MyCourseStatus ChangeType { get; set; }

        public override string GetRoutingKey()
        {
            return $"microservice.events.learner.course.{ChangeType.ToString().ToLower()}";
        }
    }
}
