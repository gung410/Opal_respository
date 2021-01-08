using Microservice.Learner.Domain.Entities;
using Thunder.Platform.Cqrs;

namespace Microservice.Learner.Application.Events
{
    public enum UserReviewEventChangeType
    {
        Created,
        Updated
    }

    public class UserReviewChangeEvent : BaseThunderEvent
    {
        public UserReviewChangeEvent(UserReview userReview, UserReviewEventChangeType changeType)
        {
            UserReview = userReview;
            ChangeType = changeType;
        }

        public UserReview UserReview { get; set; }

        public UserReviewEventChangeType ChangeType { get; set; }

        public override string GetRoutingKey()
        {
            return $"microservice.events.learner.userreview.{ChangeType.ToString().ToLower()}";
        }
    }
}
