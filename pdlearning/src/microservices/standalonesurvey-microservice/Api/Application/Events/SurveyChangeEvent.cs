using Thunder.Platform.Cqrs;

#pragma warning disable SA1402 // File may only contain a single type
namespace Microservice.StandaloneSurvey.Application.Events
{
    public enum SurveyChangeType
    {
        Created,
        Deleted,
        Updated,
        Archived,
        Rollback
    }

    public class SurveyChangeEvent : BaseThunderEvent
    {
        public SurveyChangeEvent(Domain.Entities.StandaloneSurvey survey, SurveyChangeType changeType)
        {
            Survey = survey;
            ChangeType = changeType;
        }

        public Domain.Entities.StandaloneSurvey Survey { get; }

        public SurveyChangeType ChangeType { get; }

        public override string GetRoutingKey()
        {
            return $"microservice.events.standalonesurvey.{ChangeType.ToString().ToLower()}";
        }
    }
}
#pragma warning restore SA1402 // File may only contain a single type
