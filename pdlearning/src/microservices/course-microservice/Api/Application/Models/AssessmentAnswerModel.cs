using System;
using System.Collections.Generic;
using Microservice.Course.Domain.Entities;

namespace Microservice.Course.Application.Models
{
    public class AssessmentAnswerModel
    {
        public AssessmentAnswerModel(AssessmentAnswer entity)
        {
            Id = entity.Id;
            AssessmentId = entity.AssessmentId;
            ParticipantAssignmentTrackId = entity.ParticipantAssignmentTrackId;
            UserId = entity.UserId;
            CriteriaAnswers = entity.CriteriaAnswers;
            CreatedBy = entity.CreatedBy;
            ChangedBy = entity.ChangedBy;
            SubmittedDate = entity.SubmittedDate;
            ChangedDate = entity.ChangedDate;
        }

        public Guid Id { get; set; }

        public Guid AssessmentId { get; set; }

        public Guid ParticipantAssignmentTrackId { get; set; }

        public Guid UserId { get; set; }

        public IEnumerable<AssessmentCriteriaAnswer> CriteriaAnswers { get; set; }

        public DateTime? SubmittedDate { get; set; }

        public Guid CreatedBy { get; set; }

        public Guid ChangedBy { get; set; }

        public DateTime? ChangedDate { get; set; }
    }
}
