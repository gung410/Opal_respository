using System;
using Microservice.Course.Application.Enums;

namespace Microservice.Course.Application.RequestDtos
{
    public class CreateCommentRequest
    {
        public string Content { get; set; }

        public Guid ObjectId { get; set; }

        public EntityCommentType EntityCommentType { get; set; }

        public CommentNotification? CommentNotification { get; set; }
    }
}
