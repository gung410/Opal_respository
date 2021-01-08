using System;
using Microservice.Badge.Application.Consumers.Dtos;
using Microservice.Badge.Application.Enums;

namespace Microservice.Badge.Application.Consumers.Messages
{
    public class LikeCreatedMessage
    {
        public int Id { get; set; }

        public Guid CreatedBy { get; set; }

        public DateTime CreatedAt { get; set; }

        public SourceType SourceType { get; set; }

        public SourceDto Source { get; set; }
    }
}
