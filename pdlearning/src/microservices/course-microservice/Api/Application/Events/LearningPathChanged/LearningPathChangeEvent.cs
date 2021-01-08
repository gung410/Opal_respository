using Microservice.Course.Application.Models;
using Thunder.Platform.Cqrs;

namespace Microservice.Course.Application.Events
{
    public class LearningPathChangeEvent : BaseThunderEvent
    {
        public LearningPathChangeEvent(LearningPathModel learningPath, LearningPathChangeType changeType)
        {
            LearningPath = learningPath;
            ChangeType = changeType;
        }

        public LearningPathModel LearningPath { get; }

        public LearningPathChangeType ChangeType { get; }

        public override string GetRoutingKey()
        {
            return $"microservice.events.course.learningpath.{ChangeType.ToString().ToLower()}";
        }
    }
}
