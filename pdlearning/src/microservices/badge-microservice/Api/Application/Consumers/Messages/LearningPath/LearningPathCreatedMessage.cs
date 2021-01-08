using System;

namespace Microservice.Badge.Application.Consumers.Messages
{
    public class LearningPathCreatedMessage
    {
        public Guid Id { get; init; }

        public Guid CreatedBy { get; init; }

        public DateTime CreatedDate { get; init; }
    }
}
