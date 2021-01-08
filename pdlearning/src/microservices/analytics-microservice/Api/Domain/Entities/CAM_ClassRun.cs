using System;
using System.Collections.Generic;

namespace Microservice.Analytics.Domain.Entities
{
    public partial class CAM_ClassRun
    {
        public CAM_ClassRun()
        {
            CamAssignment = new HashSet<CAM_Assignment>();
            CamClassRunFacilitator = new HashSet<CAM_ClassRunFacilitator>();
            CamLecture = new HashSet<CAM_Lecture>();
            CamRegistration = new HashSet<CAM_Registration>();
            CamSection = new HashSet<CAM_Section>();
            CamSession = new HashSet<CAM_Session>();
            LearnerUserClassRun = new HashSet<Learner_UserClassRun>();
            LearnerUserReviews = new HashSet<Learner_UserReview>();
        }

        public Guid ClassRunId { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public DateTime? DeletedDate { get; set; }

        public Guid CourseId { get; set; }

        public string ClassTitle { get; set; }

        public string ClassRunCode { get; set; }

        public DateTime? StartDateTime { get; set; }

        public DateTime? EndDateTime { get; set; }

        public string FacilitatorIds { get; set; }

        public string CoFacilitatorIds { get; set; }

        public int MinClassSize { get; set; }

        public int MaxClassSize { get; set; }

        public DateTime? ApplicationStartDate { get; set; }

        public DateTime? ApplicationEndDate { get; set; }

        public string Status { get; set; }

        public Guid CreatedBy { get; set; }

        public Guid? ChangedBy { get; set; }

        public Guid? CreatedByUserHistoryId { get; set; }

        public Guid? ChangedByUserHistoryId { get; set; }

        public string CreatedByDepartmentId { get; set; }

        public string ChangedByDepartmentId { get; set; }

        public bool IsDeleted { get; set; }

        public Guid? ClassRunVenueId { get; set; }

        public string ExternalId { get; set; }

        public string CancellationStatus { get; set; }

        public DateTime? RescheduleEndDateTime { get; set; }

        public DateTime? RescheduleStartDateTime { get; set; }

        public string RescheduleStatus { get; set; }

        public string ContentStatus { get; set; }

        public DateTime? PublishedContentDate { get; set; }

        public DateTime? SubmittedContentDate { get; set; }

        public DateTime? ApprovalContentDate { get; set; }

        public DateTime? PlanningEndTime { get; set; }

        public DateTime? PlanningStartTime { get; set; }

        public bool? CourseCriteriaActivated { get; set; }

        public virtual CAM_Course Course { get; set; }

        public virtual ICollection<CAM_Assignment> CamAssignment { get; set; }

        public virtual ICollection<CAM_ClassRunFacilitator> CamClassRunFacilitator { get; set; }

        public virtual ICollection<CAM_Lecture> CamLecture { get; set; }

        public virtual ICollection<CAM_Registration> CamRegistration { get; set; }

        public virtual ICollection<CAM_Section> CamSection { get; set; }

        public virtual ICollection<CAM_Session> CamSession { get; set; }

        public virtual ICollection<Learner_UserClassRun> LearnerUserClassRun { get; set; }

        public virtual ICollection<Learner_UserReview> LearnerUserReviews { get; set; }
    }
}
