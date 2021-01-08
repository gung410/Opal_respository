using System;

namespace Microservice.Analytics.Domain.Entities
{
    public partial class Learner_UserClassRun
    {
        public Guid UserClassRunId { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public DateTime? DeletedDate { get; set; }

        public Guid UserId { get; set; }

        public Guid UserHistoryId { get; set; }

        public Guid CourseId { get; set; }

        public Guid ClassRunId { get; set; }

        public string DepartmentId { get; set; }

        public string Status { get; set; }

        public bool IsDeleted { get; set; }

        public string WithdrawalStatus { get; set; }

        public Guid RegistrationId { get; set; }

        public string RegistrationType { get; set; }

        public Guid? AdministratedBy { get; set; }

        public Guid? ChangedBy { get; set; }

        public Guid? ClassRunChangeId { get; set; }

        public DateTime? ClassRunChangeRequestedDate { get; set; }

        public string ClassRunChangeStatus { get; set; }

        public string LearningStatus { get; set; }

        public bool? PostCourseEvaluationFormCompleted { get; set; }

        public double? Rate { get; set; }

        public DateTime? RateDate { get; set; }

        public string RateCommentTitle { get; set; }

        public string RateCommentContent { get; set; }

        public virtual CAM_ClassRun ClassRun { get; set; }

        public virtual SAM_Department Department { get; set; }

        public virtual CAM_Registration Registration { get; set; }

        public virtual SAM_UserHistory UserHistory { get; set; }
    }
}
