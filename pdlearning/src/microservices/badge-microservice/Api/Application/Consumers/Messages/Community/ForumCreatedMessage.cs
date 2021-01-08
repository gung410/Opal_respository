using System;
using Microservice.Badge.Application.Consumers.Dtos;

namespace Microservice.Badge.Application.Consumers.Messages
{
    public class ForumCreatedMessage
    {
        public int Id { get; set; }

        public Guid CreatedBy { get; set; }

        public SourceDto Source { get; set; }
    }
}
