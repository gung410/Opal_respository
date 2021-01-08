using System;
using System.Collections.Generic;
using Microservice.Course.Application.Enums;
using Thunder.Platform.Cqrs;

namespace Microservice.Course.Application.Commands
{
    public class SaveCommentCommand : BaseThunderCommand
    {
        public Guid? Id { get; set; }

        public bool IsCreate { get; set; }

        public string Content { get; set; }

        public Guid ObjectId { get; set; }

        // This field has value when user comment for many objects at same time
        public IEnumerable<Guid> ObjectIds { get; set; }

        public Enum StatusEnum { get; set; }

        public EntityCommentType EntityCommentType { get; set; }

        public CommentNotification? Notification { get; set; }
    }
}
