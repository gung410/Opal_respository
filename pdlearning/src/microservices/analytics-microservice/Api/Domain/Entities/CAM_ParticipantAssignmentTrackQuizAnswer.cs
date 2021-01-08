using System;
using System.Collections.Generic;

namespace Microservice.Analytics.Domain.Entities
{
    public partial class CAM_ParticipantAssignmentTrackQuizAnswer
    {
        public CAM_ParticipantAssignmentTrackQuizAnswer()
        {
            CamParticipantAssignmentTrackQuizQuestionAnswer = new HashSet<CAM_ParticipantAssignmentTrackQuizQuestionAnswer>();
        }

        public Guid ParticipantAssignmentTrackId { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime? ChangedDate { get; set; }

        public DateTime? DeletedDate { get; set; }

        public Guid QuizAssignmentFormId { get; set; }

        public float Score { get; set; }

        public float ScorePercentage { get; set; }

        public bool IsDeleted { get; set; }

        public virtual CAM_ParticipantAssignmentTrack ParticipantAssignmentTrack { get; set; }

        public virtual CAM_QuizAssignmentForm QuizAssignmentForm { get; set; }

        public virtual ICollection<CAM_ParticipantAssignmentTrackQuizQuestionAnswer> CamParticipantAssignmentTrackQuizQuestionAnswer { get; set; }
    }
}
