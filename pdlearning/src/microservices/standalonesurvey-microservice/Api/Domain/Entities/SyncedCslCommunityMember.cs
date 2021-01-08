using System;
using Microservice.StandaloneSurvey.Domain.ValueObjects;

namespace Microservice.StandaloneSurvey.Domain.Entities
{
    public class SyncedCslCommunityMember : BaseEntity
    {
        public SyncedCslCommunityMember(Guid communityId, Guid userId, string email, string status, string visibility, string displayName, string url, CommunityMembershipRole role, DateTime createdAt, DateTime updatedAt)
        {
            Id = Guid.NewGuid();
            CommunityId = communityId;
            UserId = userId;
            Email = email;
            Status = status;
            Visibility = visibility;
            DisplayName = displayName;
            Url = url;
            Role = role;
            CreatedAt = createdAt;
            UpdatedAt = updatedAt;
        }

        public Guid CommunityId { get; set; }

        public Guid UserId { get; set; }

        public string Email { get; set; }

        public string Status { get; set; }

        public string Visibility { get; set; }

        public string DisplayName { get; set; }

        public string Url { get; set; }

        public CommunityMembershipRole Role { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }
    }
}
