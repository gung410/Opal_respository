using System;
using Microservice.Form.Domain;
using Microservice.Form.Domain.ValueObjects.Form;
using Thunder.Platform.Core.Domain.Entities;

namespace Microservice.Form.Application.Models
{
    public class FormParticipantModel : BaseEntity, ISoftDelete
    {
        public FormParticipantModel()
        {
            // Do not remove this constructor. It is being used by JsonSerializer.Deserialize
        }

        public FormParticipantModel(Microservice.Form.Domain.Entities.FormParticipant entity)
        {
            Id = entity.Id;
            FormId = entity.FormId;
            UserId = entity.UserId;
            AssignedDate = entity.AssignedDate;
            SubmittedDate = entity.SubmittedDate;
            Status = entity.Status;
            ChangedBy = entity.ChangedBy;
            CreatedBy = entity.CreatedBy;
            IsStarted = entity.IsStarted;
        }

        public Guid FormId { get; set; }

        public Guid UserId { get; set; }

        public DateTime AssignedDate { get; set; }

        public DateTime? SubmittedDate { get; set; }

        public FormParticipantStatus Status { get; set; }

        public bool IsDeleted { get; set; }

        public bool? IsStarted { get; set; }
    }
}
