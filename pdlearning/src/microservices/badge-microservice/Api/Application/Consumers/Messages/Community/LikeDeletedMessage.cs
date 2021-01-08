using Microservice.Badge.Application.Enums;

namespace Microservice.Badge.Application.Consumers.Messages
{
    public class LikeDeletedMessage
    {
        public int Id { get; set; }

        public SourceType SourceType { get; set; }
    }
}
