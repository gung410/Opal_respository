namespace LearnerApp.Models.Achievement
{
    public class AchievementBadgeInfoDto
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public AchievementBadgeInfoLevel LevelImages { get; set; } = new AchievementBadgeInfoLevel();

        public string TagImage { get; set; }

        public class AchievementBadgeInfoLevel
        {
            public string Level1 { get; set; }

            public string Level2 { get; set; }

            public string Level3 { get; set; }
        }
    }
}
