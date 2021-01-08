using Microservice.Learner.Application.Models;
using Thunder.Platform.Cqrs;

namespace Microservice.Learner.Application.Events
{
    public enum AverageReviewRatingType
    {
        Created,
        Updated
    }

    public class AverageReviewRatingChangedEvent : BaseThunderEvent
    {
        public AverageReviewRatingChangedEvent(
            AverageReviewRatingModel averageReviewRating,
            AverageReviewRatingType averageReviewType)
        {
            AverageReviewType = averageReviewType;
            AverageReviewRating = averageReviewRating;
        }

        public AverageReviewRatingType AverageReviewType { get; set; }

        public AverageReviewRatingModel AverageReviewRating { get; set; }

        public override string GetRoutingKey()
        {
            return $"microservice.events.learner.averagereviewrating.{AverageReviewType.ToString().ToLower()}";
        }
    }
}
