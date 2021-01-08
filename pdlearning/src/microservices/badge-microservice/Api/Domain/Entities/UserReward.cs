using System;
using Microservice.Badge.Domain.Enums;
using Microservice.Badge.Domain.ValueObjects;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;
using Thunder.Platform.Core.Timing;

namespace Microservice.Badge.Domain.Entities
{
    public class UserReward
    {
        public UserReward(Guid userId, Guid badgeId)
        {
            UserId = userId;
            BadgeId = badgeId;
            IssuedDate = Clock.Now;
        }

        [BsonId(IdGenerator = typeof(GuidGenerator))]
        public Guid Id { get; init; }

        public Guid UserId { get; init; }

        public Guid BadgeId { get; init; }

        public Guid? CommunityId { get; private set; }

        public DateTime IssuedDate { get; init; }

        public Guid? IssuedBy { get; private set; }

        public BadgeLevel BadgeLevel { get; private set; }

        public UserReward SetCommunity(Guid communityId)
        {
            this.CommunityId = communityId;
            return this;
        }

        public UserReward SetIssuedBy(Guid issuedBy)
        {
            this.IssuedBy = issuedBy;
            return this;
        }

        public UserReward SetLevel(BadgeLevelEnum level)
        {
            this.BadgeLevel = new()
            {
                Level = level,
                IncreaseLevelDate = Clock.Now
            };
            return this;
        }
    }
}
