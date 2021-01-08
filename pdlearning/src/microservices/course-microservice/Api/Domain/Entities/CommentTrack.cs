using System;
using Thunder.Platform.Core.Domain.Auditing;

namespace Microservice.Course.Domain.Entities
{
    public class CommentTrack : AuditedEntity
    {
        public Guid CommentId { get; set; }

        public Guid UserId { get; set; }
    }
}
