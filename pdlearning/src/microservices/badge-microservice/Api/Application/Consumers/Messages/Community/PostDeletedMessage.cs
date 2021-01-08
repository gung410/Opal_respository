using Microservice.Badge.Application.Enums;

namespace Microservice.Badge.Application.Consumers.Messages
{
    public class PostDeletedMessage
    {
        public int Id { get; set; }

        public SourceType SourceType { get; set; }

        public bool HasContentForward { get; set; }

        public SourceType? ContentForwardType { get; set; }
    }
}
