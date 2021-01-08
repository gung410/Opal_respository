using System;
using LearnerApp.Common;

namespace LearnerApp.Models
{
    public class MyClassRun
    {
        public string Id { get; set; }

        public string UserId { get; set; }

        public string CourseId { get; set; }

        public string ClassRunId { get; set; }

        public string RegistrationId { get; set; }

        public RegistrationType RegistrationType { get; set; }

        public string Status { get; set; }

        public string WithdrawalStatus { get; set; }

        public string Comment { get; set; }

        public string Reason { get; set; }

        public DateTime ChangedDate { get; set; }

        public string ChangedBy { get; set; }

        public ClassRunChangeStatus? ClassRunChangeStatus { get; set; }

        public StatusLearning? LearningStatus { get; set; }

        public DateTime? ClassRunChangeRequestedDate { get; set; }

        public string ClassRunChangeId { get; set; }

        public bool? PostCourseEvaluationFormCompleted { get; set; }
    }
}
