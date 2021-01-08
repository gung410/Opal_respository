using System;
using Microservice.StandaloneSurvey.Domain.ValueObjects;
using StackExchange.Redis;

namespace Microservice.StandaloneSurvey.Domain.Entities
{
    public class SyncedCslCommunity : BaseEntity
    {
        public SyncedCslCommunity(
            Guid id,
            string name,
            Guid? mainCommunityId,
            string url,
            string visibility,
            string joinPolicy,
            CommunityStatus status,
            Guid createdBy,
            Guid updatedBy,
            DateTime createdAt,
            DateTime updatedAt)
        {
            Id = id;
            Name = name;
            MainCommunityId = mainCommunityId;
            Url = url;
            Visibility = visibility;
            JoinPolicy = joinPolicy;
            Status = status;
            CreatedBy = createdBy;
            UpdatedBy = updatedBy;
            CreatedAt = createdAt;
            UpdatedAt = updatedAt;
        }

        public string Name { get; private set; }

        public Guid? MainCommunityId { get; private set; }

        public string Url { get; private set; }

        public string Visibility { get; private set; }

        public string JoinPolicy { get; private set; }

        public CommunityStatus Status { get; private set; }

        public Guid UpdatedBy { get; set; }

        public DateTime CreatedAt { get; private set; }

        public DateTime UpdatedAt { get; private set; }

        public void Update(
            Guid id,
            string name,
            Guid? mainCommunityId,
            string url,
            string visibility,
            string joinPolicy,
            CommunityStatus status,
            Guid createdBy,
            Guid updatedBy,
            DateTime createdAt,
            DateTime updatedAt)
        {
            Id = id;
            Name = name;
            MainCommunityId = mainCommunityId;
            Url = url;
            Visibility = visibility;
            JoinPolicy = joinPolicy;
            Status = status;
            CreatedBy = createdBy;
            ChangedBy = updatedBy;
            CreatedAt = createdAt;
            UpdatedAt = updatedAt;
        }
    }
}
