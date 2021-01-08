using System;
using Microservice.Badge.Application.Enums;

namespace Microservice.Badge.Application.Consumers.Messages
{
    public class MicroLearningCompletedMessage
    {
        public Guid UserId { get; init; }

        public Guid CourseId { get; init; }

        public LearningStatus Status { get; init; }

        public DateTime? CompletedDate { get; init; }
    }
}
