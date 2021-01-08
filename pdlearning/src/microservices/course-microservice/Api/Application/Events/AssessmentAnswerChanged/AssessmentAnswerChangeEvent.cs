using Microservice.Course.Domain.Entities;
using Thunder.Platform.Cqrs;

namespace Microservice.Course.Application.Events
{
    public class AssessmentAnswerChangeEvent : BaseThunderEvent
    {
        public AssessmentAnswerChangeEvent(AssessmentAnswer assessmentAnswer, AssessmentAnswerChangeType changeType)
        {
            AssessmentAnswer = assessmentAnswer;
            ChangeType = changeType;
        }

        public AssessmentAnswer AssessmentAnswer { get; }

        public AssessmentAnswerChangeType ChangeType { get; }

        public bool IsMigrate { get; }

        public override string GetRoutingKey()
        {
            return $"microservice.events.course.assessmentanswer.{ChangeType.ToString().ToLower()}";
        }
    }
}
