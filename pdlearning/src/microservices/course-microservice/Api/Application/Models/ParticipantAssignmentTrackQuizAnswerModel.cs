using System;
using System.Collections.Generic;
using System.Linq;
using Microservice.Course.Domain.Entities;

namespace Microservice.Course.Application.Models
{
    public class ParticipantAssignmentTrackQuizAnswerModel
    {
        public ParticipantAssignmentTrackQuizAnswerModel(
            ParticipantAssignmentTrackQuizAnswer participantAssignmentTrackQuizAnswer,
            IEnumerable<ParticipantAssignmentTrackQuizQuestionAnswer> questionAnswers)
        {
            Id = participantAssignmentTrackQuizAnswer.Id;
            QuizAssignmentFormId = participantAssignmentTrackQuizAnswer.QuizAssignmentFormId;
            Score = participantAssignmentTrackQuizAnswer.Score;
            ScorePercentage = participantAssignmentTrackQuizAnswer.ScorePercentage;
            QuestionAnswers = questionAnswers.Select(x => new ParticipantAssignmentTrackQuizQuestionAnswerModel(x));
        }

        public Guid Id { get; set; }

        public Guid QuizAssignmentFormId { get; set; }

        public float Score { get; set; }

        public float ScorePercentage { get; set; }

        public IEnumerable<ParticipantAssignmentTrackQuizQuestionAnswerModel> QuestionAnswers { get; set; }
    }
}
