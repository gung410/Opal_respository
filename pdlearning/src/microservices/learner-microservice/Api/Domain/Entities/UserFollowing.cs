using System;
using Thunder.Platform.Core.Domain.Auditing;

namespace Microservice.Learner.Domain.Entities
{
    public class UserFollowing : AuditedEntity
    {
        public Guid UserId { get; set; }

        public Guid FollowingUserId { get; set; }

        public Guid CreatedBy { get; set; }
    }
}
