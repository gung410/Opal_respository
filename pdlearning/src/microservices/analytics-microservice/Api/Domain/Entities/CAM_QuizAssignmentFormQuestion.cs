using System;
using System.Collections.Generic;

namespace Microservice.Analytics.Domain.Entities
{
    public partial class CAM_QuizAssignmentFormQuestion
    {
        public CAM_QuizAssignmentFormQuestion()
        {
            CamParticipantAssignmentTrackQuizQuestionAnswer = new HashSet<CAM_ParticipantAssignmentTrackQuizQuestionAnswer>();
        }

        public Guid QuizAssignmentFormQuestionId { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime? ChangedDate { get; set; }

        public DateTime? DeletedDate { get; set; }

        public Guid QuizAssignmentFormId { get; set; }

        public float MaxScore { get; set; }

        public string QuestionType { get; set; }

        public string QuestionTitle { get; set; }

        public string QuestionCorrectAnswer { get; set; }

        public string QuestionOptions { get; set; }

        public string QuestionHint { get; set; }

        public string QuestionAnswerExplanatoryNote { get; set; }

        public string QuestionFeedbackCorrectAnswer { get; set; }

        public string QuestionFeedbackWrongAnswer { get; set; }

        public bool IsDeleted { get; set; }

        public int Priority { get; set; }

        public bool? RandomizedOptions { get; set; }

        public virtual CAM_QuizAssignmentForm QuizAssignmentForm { get; set; }

        public virtual ICollection<CAM_ParticipantAssignmentTrackQuizQuestionAnswer> CamParticipantAssignmentTrackQuizQuestionAnswer { get; set; }
    }
}
