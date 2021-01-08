using System;
using Microservice.Badge.Application.Enums;

namespace Microservice.Badge.Application.Consumers.Messages
{
    public class LearningPathBookMarkedMessage
    {
        public Guid Id { get; init; }

        public BookmarkType ItemType { get; init; }

        public Guid CreatedBy { get; init; }

        public DateTime CreatedDate { get; init; }
    }
}
