using System;

namespace LearnerApp.Models.Achievement
{
    public class AchievementECertificateDto
    {
        public string Id { get; set; }

        public string CourseId { get; set; }

        public DateTime LearningCompletedDate { get; set; }
    }
}
