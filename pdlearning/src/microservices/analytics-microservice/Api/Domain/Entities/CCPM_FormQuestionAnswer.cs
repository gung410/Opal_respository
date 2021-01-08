using System;

namespace Microservice.Analytics.Domain.Entities
{
    public partial class CCPM_FormQuestionAnswer
    {
        public Guid FormQuestionAnswerId { get; set; }

        public DateTime CreatedDate { get; set; }

        public Guid CreatedByUserId { get; set; }

        public Guid? CreatedByUserHistoryId { get; set; }

        public string CreatedByDepartmentId { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public Guid? UpdatedByUserId { get; set; }

        public Guid? UpdatedByUserHistoryId { get; set; }

        public string UpdatedByDepartmentId { get; set; }

        public Guid FormAnswerId { get; set; }

        public Guid FormQuestionId { get; set; }

        public string AnswerValue { get; set; }

        public double? MaxScore { get; set; }

        public double? Score { get; set; }

        public Guid? ScoredByUserId { get; set; }

        public Guid? ScoredByUserHistoryId { get; set; }

        public string ScoredByDepartmentId { get; set; }

        public string AnswerFeedback { get; set; }

        public DateTime? SubmittedDate { get; set; }

        public int? SpentTimeInSeconds { get; set; }

        public virtual CCPM_FormAnswer FormAnswer { get; set; }

        public virtual CCPM_FormQuestion FormQuestion { get; set; }
    }
}
