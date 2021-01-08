using System;

namespace Microservice.Analytics.Domain.Entities
{
    public partial class Learner_UserAssignment
    {
        public Guid UserAssignmentId { get; set; }

        public DateTime CreatedDate { get; set; }

        public int? CreatedDateKey { get; set; }

        public int? CreatedDateTime { get; set; }

        public DateTime? ChangedDate { get; set; }

        public DateTime? DeletedDate { get; set; }

        public Guid RegistrationId { get; set; }

        public Guid UserId { get; set; }

        public Guid UserHistoryId { get; set; }

        public Guid DepartmentId { get; set; }

        public Guid AssignmentId { get; set; }

        public string Status { get; set; }

        public Guid CreatedBy { get; set; }

        public Guid? ChangedBy { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime? SubmittedDate { get; set; }

        public Guid ParticipantAssignmentTrackId { get; set; }

        public virtual CAM_Assignment Assignment { get; set; }

        public virtual CAM_ParticipantAssignmentTrack ParticipantAssignmentTrack { get; set; }

        public virtual CAM_Registration Registration { get; set; }

        public virtual SAM_UserHistory UserHistory { get; set; }
    }
}
