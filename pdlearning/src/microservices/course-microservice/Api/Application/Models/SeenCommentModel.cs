using System;
using System.Collections.Generic;

namespace Microservice.Course.Application.Models
{
    public class SeenCommentModel
    {
        public Guid ObjectId { get; set; }

        public IEnumerable<Guid> CommentNotSeenIds { get; set; }
    }
}
