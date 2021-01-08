using System;
using Microservice.Content.Domain.Enums;
using Thunder.Platform.Core.Domain.Auditing;
using Thunder.Platform.Core.Domain.Entities;

namespace Microservice.Content.Domain.Entities
{
    public class AttributionElement : FullAuditedEntity, ISoftDelete
    {
        public Guid DigitalContentId { get; set; }

        public string Title { get; set; }

        public string Author { get; set; }

        public string Source { get; set; }

        public LicenseType LicenseType { get; set; }

        public bool IsDeleted { get; set; }
    }
}
