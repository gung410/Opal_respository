using System;

namespace Microservice.Analytics.Domain.Entities
{
    public partial class Learner_UserLearningPackage
    {
        public Guid UserLearningPackageId { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime? ChangedDate { get; set; }

        public Guid UserId { get; set; }

        public Guid UserHistoryId { get; set; }

        public string DepartmentId { get; set; }

        public Guid? UserLectureInCourseId { get; set; }

        public Guid? UserDigitalContentId { get; set; }

        public string Type { get; set; }

        public string State { get; set; }

        public bool IsDeleted { get; set; }

        public Guid CreatedBy { get; set; }

        public Guid? ChangedBy { get; set; }

        public string CompletionStatus { get; set; }

        public string SuccessStatus { get; set; }

        public string LessonStatus { get; set; }

        public double? ScoreScaled { get; set; }

        public double? Score { get; set; }

        public double? ScoreMax { get; set; }

        public double? ScoreMin { get; set; }

        public TimeSpan? TotalTime { get; set; }

        public TimeSpan? SessionTime { get; set; }

        public virtual Learner_UserDigitalContent UserDigitalContent { get; set; }

        public virtual Learner_LecturesInUserCourse UserLectureInCourse { get; set; }

        public virtual Learner_UserLectureInCourse UserLectureInCourseNavigation { get; set; }
    }
}
