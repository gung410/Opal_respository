using System;
using Microservice.Course.Domain.Entities;
using Microservice.Course.Domain.Enums;

namespace Microservice.Course.Application.Models
{
    public class ParticipantAssignmentTrackModel
    {
        public ParticipantAssignmentTrackModel(ParticipantAssignmentTrack entity, ParticipantAssignmentTrackQuizAnswer quizAnswer = null)
        {
            Id = entity.Id;
            RegistrationId = entity.RegistrationId;
            AssignmentId = entity.AssignmentId;
            UserId = entity.UserId;
            AssignedDate = entity.AssignedDate;
            SubmittedDate = entity.SubmittedDate;
            StartDate = entity.StartDate;
            EndDate = entity.EndDate;
            Status = entity.Status;
            ChangedBy = entity.ChangedBy;
            CreatedBy = entity.CreatedBy;
            QuizAnswer = quizAnswer == null ? null : new ParticipantAssignmentTrackQuizAnswerModel(quizAnswer, quizAnswer.QuestionAnswers);
        }

        public Guid Id { get; set; }

        public Guid RegistrationId { get; set; }

        public Guid AssignmentId { get; set; }

        public Guid UserId { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public DateTime AssignedDate { get; set; }

        public DateTime? SubmittedDate { get; set; }

        public ParticipantAssignmentTrackStatus Status { get; set; }

        public Guid CreatedBy { get; set; }

        public Guid ChangedBy { get; set; }

        public ParticipantAssignmentTrackQuizAnswerModel QuizAnswer { get; set; }
    }
}
