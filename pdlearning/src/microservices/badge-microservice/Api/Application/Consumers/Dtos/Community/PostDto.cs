using System;
using Microservice.Badge.Application.Enums;

namespace Microservice.Badge.Application.Consumers.Dtos
{
    public class PostDto
    {
        public int Id { get; set; }

        public string Message { get; set; }

        public Guid CreatedBy { get; set; }

        public SourceType SourceType { get; set; }

        public SourceDto Source { get; set; }

        public bool HasContentForward { get; set; }

        public SourceType? ContentForwardType { get; set; }

        public PostDto ContentForward { get; set; }
    }
}
