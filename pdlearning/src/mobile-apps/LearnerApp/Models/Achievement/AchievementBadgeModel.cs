using System;

namespace LearnerApp.Models.Achievement
{
    public class AchievementBadgeModel
    {
        public AchievementBadgeModel(AchievementUserBadgeDto dto, AchievementBadgeInfoDto info, string communityName)
            : this(dto, info)
        {
            AdditionalInfo = communityName;
        }

        public AchievementBadgeModel(AchievementUserBadgeDto dto, AchievementBadgeInfoDto info)
        {
            BadgeName = info.Name;
            IssuedDate = dto.IssuedDate;

            if (info.LevelImages != null)
            {
                switch (dto.BadgeLevel.Level)
                {
                    case "Level1":
                        BadgeImageUrl = info.LevelImages.Level1;
                        break;

                    case "Level2":
                        BadgeImageUrl = info.LevelImages.Level2;
                        break;

                    case "Level3":
                        BadgeImageUrl = info.LevelImages.Level3;
                        break;
                }
            }
            else
            {
                BadgeImageUrl = info.TagImage;
            }
        }

        public string BadgeName { get; set; }

        public string BadgeImageUrl { get; set; }

        public DateTime IssuedDate { get; set; }

        public string AdditionalInfo { get; set; }
    }
}
