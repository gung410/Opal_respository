using Microservice.Learner.Application.Models;
using Thunder.Platform.Cqrs;

namespace Microservice.Learner.Application.Events
{
    public enum LearningProcessType
    {
        Created,
        Updated
    }

    public class LearningProcessChangeEvent : BaseThunderEvent
    {
        public LearningProcessChangeEvent(LearningProcessModel learningProcess, LearningProcessType changeType)
        {
            LearningProcess = learningProcess;
            ChangeType = changeType;
        }

        public LearningProcessModel LearningProcess { get; set; }

        public LearningProcessType ChangeType { get; set; }

        public override string GetRoutingKey()
        {
            return $"microservice.events.learner.process.{ChangeType.ToString().ToLower()}";
        }
    }
}
