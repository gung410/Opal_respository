using System;
using Microservice.Content.Domain.ValueObject;

namespace Microservice.Content.Application.RequestDtos
{
    public class CreateVideoCommentRequest
    {
        public Guid? Id { get; set; } = Guid.Empty;

        public Guid? ObjectId { get; set; } = Guid.Empty;

        public Guid? OriginalObjectId { get; set; } = Guid.Empty;

        public VideoSourceType SourceType { get; set; }

        public string Content { get; set; }

        public Guid VideoId { get; set; }

        public int VideoTime { get; set; }
    }
}
