using System;
using System.Collections.Generic;

namespace Microservice.Analytics.Domain.Entities
{
    public partial class CCPM_FormQuestion
    {
        public CCPM_FormQuestion()
        {
            CcpmFormQuestionAnswer = new HashSet<CCPM_FormQuestionAnswer>();
        }

        public Guid FormQuestionsId { get; set; }

        public DateTime CreatedDate { get; set; }

        public Guid CreatedByUserId { get; set; }

        public Guid? CreatedByUserHistoryId { get; set; }

        public string CreatedByDepartmentId { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public Guid? UpdatedByUserId { get; set; }

        public Guid? UpdatedByUserHistoryId { get; set; }

        public string UpdatedByDepartmentId { get; set; }

        public string QuestionType { get; set; }

        public string QuestionTitle { get; set; }

        public string QuestionCorrectAnswer { get; set; }

        public string QuestionOptions { get; set; }

        public string QuestionHint { get; set; }

        public string QuestionAnswerExplanatoryNote { get; set; }

        public Guid FormId { get; set; }

        public int Priority { get; set; }

        public double? Score { get; set; }

        public bool IsDeleted { get; set; }

        public string QuestionFeedbackCorrectAnswer { get; set; }

        public string QuestionFeedbackWrongAnswer { get; set; }

        public Guid? ParentId { get; set; }

        public Guid? QuestionNextQuestionId { get; set; }

        public virtual CCPM_Form Form { get; set; }

        public virtual ICollection<CCPM_FormQuestionAnswer> CcpmFormQuestionAnswer { get; set; }
    }
}
