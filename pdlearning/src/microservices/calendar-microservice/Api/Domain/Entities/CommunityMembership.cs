using System;
using Microservice.Calendar.Domain.Enums;
using Thunder.Platform.Core.Domain.Auditing;

namespace Microservice.Calendar.Domain.Entities
{
    public class CommunityMembership : FullAuditedEntity
    {
        public Guid CommunityId { get; set; }

        public Guid UserId { get; set; }

        public CommunityMembershipRole Role { get; set; }
    }
}
