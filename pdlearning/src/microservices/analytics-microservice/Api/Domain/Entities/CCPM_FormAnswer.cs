using System;
using System.Collections.Generic;

namespace Microservice.Analytics.Domain.Entities
{
    public partial class CCPM_FormAnswer
    {
        public CCPM_FormAnswer()
        {
            CcpmFormQuestionAnswer = new HashSet<CCPM_FormQuestionAnswer>();
        }

        public Guid FormAnswerId { get; set; }

        public Guid FormId { get; set; }

        public Guid UserId { get; set; }

        public Guid? UserHistoryId { get; set; }

        public string DepartmentId { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public Guid? UpdatedByUserId { get; set; }

        public Guid? UpdatedByUserHistoryId { get; set; }

        public string UpdatedByDepartmentId { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public DateTime? SubmitDate { get; set; }

        public double? Score { get; set; }

        public double? ScorePercentage { get; set; }

        public short Attempt { get; set; }

        public string FormMetaData { get; set; }

        public bool IsDeleted { get; set; }

        public bool IsCompleted { get; set; }

        public Guid? CourseId { get; set; }

        public Guid? AssignmentId { get; set; }

        public Guid? ClassRunId { get; set; }

        public double? Satisfaction { get; set; }

        public double? SqRating { get; set; }

        public double? Usefulness { get; set; }

        public bool? Status { get; set; }

        public virtual CCPM_Form Form { get; set; }

        public virtual SAM_UserHistory UserHistory { get; set; }

        public virtual ICollection<CCPM_FormQuestionAnswer> CcpmFormQuestionAnswer { get; set; }
    }
}
