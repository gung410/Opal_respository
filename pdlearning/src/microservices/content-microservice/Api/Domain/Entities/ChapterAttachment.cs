using System;
using Thunder.Platform.Core.Domain.Auditing;
using Thunder.Platform.Core.Domain.Entities;

namespace Microservice.Content.Domain.Entities
{
    public class ChapterAttachment : AuditedEntity, ISoftDelete
    {
        public Guid ObjectId { get; set; }

        public string FileLocation { get; set; }

        public string FileName { get; set; }

        public bool IsDeleted { get; set; }

        public virtual Chapter Chapter { get; set; }
    }
}
