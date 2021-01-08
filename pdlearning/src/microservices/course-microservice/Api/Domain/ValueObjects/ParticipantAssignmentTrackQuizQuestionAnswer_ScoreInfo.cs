using System;

namespace Microservice.Course.Domain.ValueObjects
{
    public class ParticipantAssignmentTrackQuizQuestionAnswer_ScoreInfo
    {
        public float Score { get; set; }

        public Guid? ManualScoredBy { get; set; }

        public Guid QuizAssignmentFormQuestionId { get; set; }
    }
}
