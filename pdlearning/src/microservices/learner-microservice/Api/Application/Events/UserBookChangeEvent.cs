using Microservice.Learner.Domain.Entities;
using Thunder.Platform.Cqrs;

namespace Microservice.Learner.Application.Events
{
    public enum UserBookmarkEventChangeType
    {
        Created,
        Deleted
    }

    public class UserBookChangeEvent : BaseThunderEvent
    {
        public UserBookChangeEvent(UserBookmark userBookmark, UserBookmarkEventChangeType changeType)
        {
            UserBookmark = userBookmark;
            ChangeType = changeType;
        }

        public UserBookmark UserBookmark { get; set; }

        public UserBookmarkEventChangeType ChangeType { get; set; }

        public override string GetRoutingKey()
        {
            return $"microservice.events.learner.bookmark.{ChangeType.ToString().ToLower()}";
        }
    }
}
