using System;
using System.Collections.Generic;
using Microservice.Calendar.Domain.Enums;
using Thunder.Platform.Core.Domain.Auditing;

namespace Microservice.Calendar.Domain.Entities
{
    public class Community : FullAuditedEntity
    {
        public string Title { get; set; }

        public Guid? ParentId { get; set; }

        public CommunityStatus Status { get; set; }

        public virtual Community Parent { get; set; }

        public virtual ICollection<CommunityEvent> CommunityEvents { get; set; }
    }
}
