using Microservice.Course.Application.Models;
using Thunder.Platform.Cqrs;

namespace Microservice.Course.Application.Events
{
    public class LectureChangeEvent : BaseThunderEvent
    {
        public LectureChangeEvent(LectureModel lectureModel, LectureChangeType changeType, bool isMigrate = false)
        {
            LectureModel = lectureModel;
            ChangeType = changeType;
            IsMigrate = isMigrate;
        }

        public LectureModel LectureModel { get; }

        public LectureChangeType ChangeType { get; }

        public bool IsMigrate { get; }

        public override string GetRoutingKey()
        {
            return $"microservice.events.course.lecture.{ChangeType.ToString().ToLower()}";
        }
    }
}
