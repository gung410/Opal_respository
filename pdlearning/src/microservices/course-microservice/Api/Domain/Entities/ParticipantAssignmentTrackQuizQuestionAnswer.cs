using System;
using System.Text.Json.Serialization;
using Thunder.Platform.Core.Domain.Entities;

namespace Microservice.Course.Domain.Entities
{
    public class ParticipantAssignmentTrackQuizQuestionAnswer : ISoftDelete
    {
        public int Id { get; set; }

        public Guid QuizAssignmentFormQuestionId { get; set; }

        public object AnswerValue { get; set; }

        public float? ManualScore { get; set; }

        public Guid? ManualScoredBy { get; set; }

        public float? Score { get; set; }

        public DateTime? SubmittedDate { get; set; }

        public Guid QuizAnswerId { get; set; }

        public bool IsDeleted { get; set; }

        [JsonIgnore]
        public virtual ParticipantAssignmentTrackQuizAnswer QuizAnswer { get; set; }
    }
}
