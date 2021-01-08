using System;
using Microservice.Course.Domain.Entities;

namespace Microservice.Course.Application.Models
{
    public class ParticipantAssignmentTrackQuizQuestionAnswerModel
    {
        public ParticipantAssignmentTrackQuizQuestionAnswerModel(ParticipantAssignmentTrackQuizQuestionAnswer questionAnswer)
        {
            Id = questionAnswer.Id;
            QuizAssignmentFormQuestionId = questionAnswer.QuizAssignmentFormQuestionId;
            QuizAnswerId = questionAnswer.QuizAnswerId;
            AnswerValue = questionAnswer.AnswerValue;
            ManualScore = questionAnswer.ManualScore;
            ManualScoredBy = questionAnswer.ManualScoredBy;
            Score = questionAnswer.Score;
            SubmittedDate = questionAnswer.SubmittedDate;
        }

        public int Id { get; set; }

        public Guid QuizAssignmentFormQuestionId { get; set; }

        public Guid QuizAnswerId { get; set; }

        public object AnswerValue { get; set; }

        public float? ManualScore { get; set; }

        public Guid? ManualScoredBy { get; set; }

        public float? Score { get; set; }

        public DateTime? SubmittedDate { get; set; }
    }
}
