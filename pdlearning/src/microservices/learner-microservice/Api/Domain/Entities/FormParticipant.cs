using System;
using Microservice.Learner.Domain.ValueObject;
using Thunder.Platform.Core.Domain.Auditing;

namespace Microservice.Learner.Domain.Entities
{
    /// <summary>
    /// Sync from Form table on the FormParticipant module.
    /// </summary>
    public class FormParticipant : AuditedEntity
    {
        public Guid UserId { get; set; }

        public Guid FormOriginalObjectId { get; set; }

        public Guid FormId { get; set; }

        public FormParticipantStatus Status { get; set; }

        public void Update(
            Guid userId,
            Guid formOriginalObjectId,
            Guid formId,
            FormParticipantStatus status,
            DateTime? changedDate)
        {
            UserId = userId;
            FormOriginalObjectId = formOriginalObjectId;
            FormId = formId;
            Status = status;
            ChangedDate = changedDate;
        }

        public bool IsCompleted()
        {
            return Status == FormParticipantStatus.Completed;
        }
    }
}
