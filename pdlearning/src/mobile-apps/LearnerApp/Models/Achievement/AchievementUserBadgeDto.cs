using System;

namespace LearnerApp.Models.Achievement
{
    public class AchievementUserBadgeDto
    {
        public string CommunityId { get; set; }

        public string BadgeId { get; set; }

        public DateTime IssuedDate { get; set; }

        public AchievementUserBadgeLevelDto BadgeLevel { get; set; }

        public class AchievementUserBadgeLevelDto
        {
            public string Level { get; set; }
        }
    }
}
