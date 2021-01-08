using System;
using System.Text.Json.Serialization;
using Microservice.StandaloneSurvey.Domain.ValueObjects;

namespace Microservice.StandaloneSurvey.Application.Consumers
{
    public class CslUserCommunityChangeMessage
    {
        public CslUserMessage User { get; set; }

        public CslCommunityMessage Community { get; set; }

        public CommunityMembershipRole Role { get; set; }

        public long Status { get; set; }

        public long CanCancelMembership { get; set; }

        public long SendNotifications { get; set; }

        public long ShowAtDashboard { get; set; }

        public CslUserMessage OriginatorUser { get; set; }

        public DateTime MemberSince { get; set; }

        public DateTime UpdatedAt { get; set; }

        public DateTime LastVisit { get; set; }
    }

    public class CslCommunityMessage
    {
        public Guid Id { get; set; }

        public Guid Guid { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string Url { get; set; }

        public string Visibility { get; set; }

        public string JoinPolicy { get; set; }

        public CommunityStatus Status { get; set; }

        public object Tags { get; set; }

        public Guid CreatedBy { get; set; }

        public Guid UpdatedBy { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public Guid? MainCommunityId { get; set; }

        public Guid? MainCommunityGuid { get; set; }
    }

    public class CslUserMessage
    {
        public long Id { get; set; }

        [JsonPropertyName("guid")]
        public Guid UserId { get; set; }

        public string Email { get; set; }

        public string Status { get; set; }

        public string Visibility { get; set; }

        public string DisplayName { get; set; }

        public string Url { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }
    }
}
