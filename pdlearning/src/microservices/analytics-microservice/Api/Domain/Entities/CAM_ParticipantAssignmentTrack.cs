using System;
using System.Collections.Generic;

namespace Microservice.Analytics.Domain.Entities
{
    public partial class CAM_ParticipantAssignmentTrack
    {
        public CAM_ParticipantAssignmentTrack()
        {
            LearnerUserAssignments = new HashSet<Learner_UserAssignment>();
        }

        public Guid ParticipantAssignmentTrackId { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime? ChangedDate { get; set; }

        public DateTime? DeletedDate { get; set; }

        public Guid RegistrationId { get; set; }

        public Guid AssignmentId { get; set; }

        public DateTime AssignedDate { get; set; }

        public DateTime? SubmittedDate { get; set; }

        public Guid CreatedBy { get; set; }

        public Guid ChangedBy { get; set; }

        public Guid? CreatedByUserHistoryId { get; set; }

        public Guid? ChangedByUserHistoryId { get; set; }

        public string CreatedByDepartmentId { get; set; }

        public string ChangedByDepartmentId { get; set; }

        public bool IsDeleted { get; set; }

        public Guid UserId { get; set; }

        public Guid UserHistoryId { get; set; }

        public Guid? DepartmentId { get; set; }

        public string Status { get; set; }

        public virtual CAM_ParticipantAssignmentTrackQuizAnswer CamParticipantAssignmentTrackQuizAnswer { get; set; }

        public virtual ICollection<Learner_UserAssignment> LearnerUserAssignments { get; set; }
    }
}
