using System;
using Thunder.Platform.Core.Domain.Auditing;
using Thunder.Platform.Core.Domain.Entities;

namespace Microservice.Learner.Domain.Entities
{
    public class UserSharingDetail : FullAuditedEntity, ISoftDelete
    {
        public Guid UserSharingId { get; set; }

        public Guid UserId { get; set; }

        public Guid CreatedBy { get; set; }

        public bool IsDeleted { get; set; }
    }
}
