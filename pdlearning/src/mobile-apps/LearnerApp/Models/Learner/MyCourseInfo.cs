using System;
using LearnerApp.Common;

namespace LearnerApp.Models
{
    public class MyCourseInfo
    {
        public string Id { get; set; }

        public string CourseId { get; set; }

        public string Version { get; set; }

        public string UserId { get; set; }

        public string Status { get; set; }

        public string ReviewStatus { get; set; }

        public double ProgressMeasure { get; set; }

        public string CurrentLecture { get; set; }

        public DateTime LastLogin { get; set; }

        public DateTime DisenrollUtc { get; set; }

        public DateTime ReadDate { get; set; }

        public DateTime ReminderSentDate { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public DateTime CompletedDate { get; set; }

        public DateTime CreatedDate { get; set; }

        public string CreatedBy { get; set; }

        public DateTime ChangedDate { get; set; }

        public string ChangedBy { get; set; }

        public string CourseType { get; set; }

        public string ResultId { get; set; }

        public string MyRegistrationStatus { get; set; }

        public string MyWithdrawalStatus { get; set; }

        public MyCourseDisplayStatus DisplayStatus { get; set; }

        public bool HasContentChanged { get; set; }

        public bool? PostCourseEvaluationFormCompleted { get; set; }
    }
}
