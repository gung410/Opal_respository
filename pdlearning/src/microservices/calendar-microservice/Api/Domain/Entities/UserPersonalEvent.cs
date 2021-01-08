using System;
using Thunder.Platform.Core.Domain.Auditing;

namespace Microservice.Calendar.Domain.Entities
{
    public class UserPersonalEvent : FullAuditedEntity
    {
        public Guid UserId { get; set; }

        public Guid EventId { get; set; }

        public bool IsAccepted { get; set; } = true;

        public virtual PersonalEvent Event { get; set; }
    }
}
