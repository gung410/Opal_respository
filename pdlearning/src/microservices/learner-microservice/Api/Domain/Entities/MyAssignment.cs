using System;
using Microservice.Learner.Domain.ValueObject;
using Thunder.Platform.Core.Domain.Auditing;
using Thunder.Platform.Core.Domain.Entities;

namespace Microservice.Learner.Domain.Entities
{
    /// <summary>
    /// Sync from the ParticipantAssignmentTrack table on the CAM module.
    /// </summary>
    public class MyAssignment : FullAuditedEntity, ISoftDelete
    {
        public Guid RegistrationId { get; set; }

        public Guid UserId { get; set; }

        public Guid AssignmentId { get; set; }

        public MyAssignmentStatus Status { get; set; }

        public DateTime? SubmittedDate { get; set; }

        public Guid CreatedBy { get; set; }

        public Guid? ChangedBy { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public bool NotCompleted()
        {
            return Status == MyAssignmentStatus.NotStarted || Status == MyAssignmentStatus.InProgress;
        }
    }
}
