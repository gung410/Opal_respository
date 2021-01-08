using System;
using System.Collections.Generic;

namespace Microservice.Analytics.Domain.Entities
{
    public partial class CAM_Registration
    {
        public CAM_Registration()
        {
            CamAttendanceTracking = new HashSet<CAM_AttendanceTracking>();
            LearnerUserAssignments = new HashSet<Learner_UserAssignment>();
            LearnerUserClassRun = new HashSet<Learner_UserClassRun>();
            LearnerUserCourses = new HashSet<Learner_UserCourse>();
        }

        public Guid RegistrationId { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime? ChangedDate { get; set; }

        public DateTime? DeletedDate { get; set; }

        public Guid UserId { get; set; }

        public Guid? UserHistoryId { get; set; }

        public Guid CourseId { get; set; }

        public Guid ClassRunId { get; set; }

        public string RegistrationType { get; set; }

        public DateTime? RegistrationDate { get; set; }

        public string Status { get; set; }

        public DateTime? LastStatusChangedDate { get; set; }

        public string WithdrawalStatus { get; set; }

        public DateTime? WithdrawalRequestDate { get; set; }

        public string ClassRunChangeStatus { get; set; }

        public DateTime? ClassRunChangeRequestedDate { get; set; }

        public Guid? ClassRunChangeId { get; set; }

        public Guid ApprovingOfficer { get; set; }

        public Guid? AlternativeApprovingOfficer { get; set; }

        public DateTime? ApprovingDate { get; set; }

        public Guid? AdministratedBy { get; set; }

        public Guid? AdministratedByUserHistoryId { get; set; }

        public string AdministratedByDepartmentId { get; set; }

        public DateTime? AdministrationDate { get; set; }

        public Guid CreatedBy { get; set; }

        public Guid? CreatedByUserHistoryId { get; set; }

        public string CreatedByDepartmentId { get; set; }

        public string ExternalId { get; set; }

        public bool IsDeleted { get; set; }

        public Guid? ChangedBy { get; set; }

        public Guid? ChangedByUserHistoryId { get; set; }

        public string ChangedByDepartmentId { get; set; }

        public double? LearningContentProgress { get; set; }

        public string LearningStatus { get; set; }

        public bool? PostCourseEvaluationFormCompleted { get; set; }

        public DateTime? OfferSentDate { get; set; }

        public DateTime? LearningCompletedDate { get; set; }

        public bool? CourseCriteriaOverrided { get; set; }

        public bool? CourseCriteriaViolated { get; set; }

        public string CourseCriteriaViolation { get; set; }

        public Guid? DepartmentId { get; set; }

        public virtual CAM_ClassRun ClassRun { get; set; }

        public virtual CAM_Course Course { get; set; }

        public virtual SAM_UserHistory UserHistory { get; set; }

        public virtual ICollection<CAM_AttendanceTracking> CamAttendanceTracking { get; set; }

        public virtual ICollection<Learner_UserAssignment> LearnerUserAssignments { get; set; }

        public virtual ICollection<Learner_UserClassRun> LearnerUserClassRun { get; set; }

        public virtual ICollection<Learner_UserCourse> LearnerUserCourses { get; set; }
    }
}
