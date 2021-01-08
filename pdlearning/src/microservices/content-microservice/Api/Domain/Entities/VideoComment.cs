using System;
using Microservice.Content.Domain.ValueObject;
using Thunder.Platform.Core.Domain.Auditing;
using Thunder.Platform.Core.Domain.Entities;

namespace Microservice.Content.Domain.Entities
{
    public class VideoComment : AuditedEntity, ISoftDelete
    {
        public Guid UserId { get; set; }

        public Guid? ObjectId { get; set; } = Guid.Empty;

        public Guid? OriginalObjectId { get; set; } = Guid.Empty;

        public VideoSourceType SourceType { get; set; }

        public string Content { get; set; }

        public Guid VideoId { get; set; }

        public int VideoTime { get; set; }

        public string Note { get; set; }

        public bool IsDeleted { get; set; }
    }
}
