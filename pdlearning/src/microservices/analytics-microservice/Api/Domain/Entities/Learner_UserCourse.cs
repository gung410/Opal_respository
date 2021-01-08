using System;
using System.Collections.Generic;

namespace Microservice.Analytics.Domain.Entities
{
    public partial class Learner_UserCourse
    {
        public Learner_UserCourse()
        {
            LearnerLecturesInUserCourse = new HashSet<Learner_LecturesInUserCourse>();
        }

        public Guid UserCourseId { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime? UpdateDate { get; set; }

        public Guid CourseId { get; set; }

        public string Version { get; set; }

        public Guid UserId { get; set; }

        public Guid UserHistoryId { get; set; }

        public string DepartmentId { get; set; }

        public string Status { get; set; }

        public string ReviewStatus { get; set; }

        public double? ProgressMeasure { get; set; }

        public DateTime? LastLogin { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime? DisenrollUtc { get; set; }

        public DateTime? ReadDate { get; set; }

        public DateTime? ReminderSentDate { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public DateTime? CompletedDate { get; set; }

        public Guid CreatedBy { get; set; }

        public Guid? ChangedBy { get; set; }

        public Guid? CurrentLecture { get; set; }

        public string CourseType { get; set; }

        public string RegistrationStatus { get; set; }

        public Guid? ResultId { get; set; }

        public string WithdrawalStatus { get; set; }

        public string ExternalId { get; set; }

        public string DisplayStatus { get; set; }

        public Guid? ClassRunId { get; set; }

        public bool? HasContentChanged { get; set; }

        public Guid? RegistrationId { get; set; }

        public bool? PostCourseEvaluationFormCompleted { get; set; }

        public decimal? CourseFee { get; set; }

        public decimal? ActualTimeSpent { get; set; }

        public double? Rate { get; set; }

        public DateTime? RateDate { get; set; }

        public string RateCommentTitle { get; set; }

        public string RateCommentContent { get; set; }

        public virtual CAM_Course Course { get; set; }

        public virtual SAM_Department Department { get; set; }

        public virtual CAM_Registration Registration { get; set; }

        public virtual SAM_UserHistory UserHistory { get; set; }

        public virtual ICollection<Learner_LecturesInUserCourse> LearnerLecturesInUserCourse { get; set; }
    }
}
