using Microservice.Badge.Application.Enums;

namespace Microservice.Badge.Application.Consumers.Messages
{
    public class CommentDeletedMessage
    {
        public int Id { get; set; }

        public ThreadType ThreadType { get; set; }
    }
}
