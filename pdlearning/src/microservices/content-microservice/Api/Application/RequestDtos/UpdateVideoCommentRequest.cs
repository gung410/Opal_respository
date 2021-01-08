using System;
using Microservice.Content.Domain.ValueObject;

namespace Microservice.Content.Application.RequestDtos
{
    public class UpdateVideoCommentRequest
    {
        public Guid Id { get; set; }

        public string Content { get; set; }

        public int VideoTime { get; set; }
    }
}
