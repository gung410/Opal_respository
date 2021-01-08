using LearnerApp.Common;

namespace LearnerApp.Models.Course
{
    public class MyCourseStatus
    {
        public string Status { get; set; }

        public string MyRegistrationStatus { get; set; }

        public string MyWithdrawalStatus { get; set; }

        public string MyCourseDisplayStatus { get; set; }

        public string LearningStatus { get; set; }

        public bool IsNominated { get; set; }

        public bool IsAddToPlan { get; set; }

        public bool IsMicroLearningType { get; set; } = true;

        public bool IsVisibleLearningStatus { get; set; } = true;

        public bool IsCourseCompleted { get; set; }

        public bool IsTableOfContentEmpty { get; set; }

        public ClassRunStatus ClassRunStatus { get; set; } = ClassRunStatus.None;
    }
}
