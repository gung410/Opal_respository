using System;
using Microservice.StandaloneSurvey.Domain;
using Microservice.StandaloneSurvey.Domain.Entities;
using Microservice.StandaloneSurvey.Domain.ValueObjects.Survey;
using Thunder.Platform.Core.Domain.Entities;

namespace Microservice.StandaloneSurvey.Application.Models
{
    public class SurveyParticipantModel : BaseEntity, ISoftDelete
    {
        public SurveyParticipantModel()
        {
            // Do not remove this constructor. It is being used by JsonSerializer.Deserialize
        }

        public SurveyParticipantModel(SurveyParticipant entity)
        {
            Id = entity.Id;
            FormId = entity.SurveyId;
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

        public SurveyParticipantStatus Status { get; set; }

        public bool IsDeleted { get; set; }

        public bool? IsStarted { get; set; }
    }
}
