using System;
using Microservice.Learner.Domain.ValueObject;
using Thunder.Platform.Core.Domain.Auditing;
using Thunder.Platform.Core.Domain.Entities;

namespace Microservice.Learner.Domain.Entities
{
    public class UserSharing : FullAuditedEntity, ISoftDelete
    {
        public Guid ItemId { get; set; }

        public SharingType ItemType { get; set; }

        public Guid CreatedBy { get; set; }

        public bool IsDeleted { get; set; }
    }
}
