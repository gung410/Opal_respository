using System;
using System.Collections.Generic;
using Microservice.Content.Domain.ValueObject;
using Thunder.Platform.Core.Domain.Auditing;
using Thunder.Platform.Core.Domain.Entities;

namespace Microservice.Content.Domain.Entities
{
    public class Chapter : AuditedEntity, ISoftDelete
    {
        public Chapter()
        {
            Attachments = new HashSet<ChapterAttachment>();
        }

        public Guid ObjectId { get; set; }

        public Guid OriginalObjectId { get; set; }

        public VideoSourceType SourceType { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public int TimeStart { get; set; }

        public int TimeEnd { get; set; }

        public string Note { get; set; }

        public Guid? CreatedBy { get; set; }

        public Guid? ChangedBy { get; set; }

        public bool IsDeleted { get; set; }

        public virtual ICollection<ChapterAttachment> Attachments { get; set; }
    }
}
