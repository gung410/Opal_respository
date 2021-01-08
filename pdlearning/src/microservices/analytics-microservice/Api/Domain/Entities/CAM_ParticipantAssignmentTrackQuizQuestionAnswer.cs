using System;

namespace Microservice.Analytics.Domain.Entities
{
    public partial class CAM_ParticipantAssignmentTrackQuizQuestionAnswer
    {
        public int ParticipantAssignmentTrackQuizQuestionAnswerId { get; set; }

        public Guid QuizAssignmentFormQuestionId { get; set; }

        public string AnswerValue { get; set; }

        public float? ManualScore { get; set; }

        public Guid? ManualScoredBy { get; set; }

        public float? Score { get; set; }

        public DateTime? SubmittedDate { get; set; }

        public Guid ParticipantAssignmentTrackId { get; set; }

        public bool IsDeleted { get; set; }

        public virtual CAM_ParticipantAssignmentTrackQuizAnswer ParticipantAssignmentTrack { get; set; }

        public virtual CAM_QuizAssignmentFormQuestion QuizAssignmentFormQuestion { get; set; }
    }
}
