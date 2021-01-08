using System;
using Microservice.Badge.Application.Consumers.Dtos;
using Microservice.Badge.Application.Enums;

namespace Microservice.Badge.Application.Consumers.Messages
{
    public class CommentCreatedMessage
    {
        public int Id { get; set; }

        public Guid CreatedBy { get; set; }

        public DateTime CreatedAt { get; set; }

        public ThreadType ThreadType { get; set; }

        public ThreadDto Thread { get; set; }
    }
}
