using System;
using System.Collections.Generic;

namespace Microservice.Analytics.Domain.Entities
{
    public partial class CAM_QuizAssignmentForm
    {
        public CAM_QuizAssignmentForm()
        {
            CamParticipantAssignmentTrackQuizAnswer = new HashSet<CAM_ParticipantAssignmentTrackQuizAnswer>();
            CamQuizAssignmentFormQuestion = new HashSet<CAM_QuizAssignmentFormQuestion>();
        }

        public Guid QuizAssignmentFormId { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime? ChangedDate { get; set; }

        public DateTime? DeletedDate { get; set; }

        public bool IsDeleted { get; set; }

        public bool RandomizedQuestions { get; set; }

        public virtual ICollection<CAM_ParticipantAssignmentTrackQuizAnswer> CamParticipantAssignmentTrackQuizAnswer { get; set; }

        public virtual ICollection<CAM_QuizAssignmentFormQuestion> CamQuizAssignmentFormQuestion { get; set; }
    }
}
