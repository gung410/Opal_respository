using Microservice.Course.Domain.Entities;
using Thunder.Platform.Cqrs;

namespace Microservice.Course.Application.Events
{
    public class CommentChangeEvent : BaseThunderEvent
    {
        public CommentChangeEvent(Comment comment, CommentChangeType changeType)
        {
            Comment = comment;
            ChangeType = changeType;
        }

        public Comment Comment { get; }

        public CommentChangeType ChangeType { get; }

        public override string GetRoutingKey()
        {
            return $"microservice.events.course.comment.{ChangeType.ToString().ToLower()}";
        }
    }
}
