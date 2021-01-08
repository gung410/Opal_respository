using System;
using Microservice.Learner.Domain.ValueObject;
using Thunder.Platform.Core.Domain.Auditing;

namespace Microservice.Learner.Domain.Entities
{
    /// <summary>
    /// Sync from DigitalContent table on the CCPM module.
    /// </summary>
    public class DigitalContent : AuditedEntity
    {
        public string Title { get; set; }

        public string Description { get; set; }

        public ContentType Type { get; set; }

        public DigitalContentStatus Status { get; set; }

        public Guid OriginalObjectId { get; set; }

        public Guid OwnerId { get; set; }

        public string FileExtension { get; set; }
    }
}
