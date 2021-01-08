using System;
using Microservice.Badge.Domain.Entities;
using Microservice.Badge.Domain.ValueObjects;

namespace Microservice.Badge.Application.Models
{
    public class UserBadgeModel
    {
        public UserBadgeModel()
        {
        }

        // Only use when get data in memory.
        public UserBadgeModel(UserReward userBadge, BadgeEntity badge)
        {
            BadgeId = userBadge.BadgeId;
            Name = badge.Name;
            Image = badge.Type == Domain.Enums.BadgeType.Tag
                ? badge.TagImage
                : badge.LevelImages[userBadge.BadgeLevel.Level];
            IssuedDate = userBadge.IssuedDate;
            IssuedBy = userBadge.IssuedBy;
            BadgeLevel = userBadge.BadgeLevel;
        }

        public Guid BadgeId { get; set; }

        public Guid? CommunityId { get; set; }

        public Guid? IssuedBy { get; set; }

        public DateTime IssuedDate { get; set; }

        public BadgeLevel BadgeLevel { get; set; }

        public string Name { get; set; }

        public string Image { get; set; }
    }
}
