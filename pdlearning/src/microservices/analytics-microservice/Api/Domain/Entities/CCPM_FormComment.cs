using System;

namespace Microservice.Analytics.Domain.Entities
{
    public partial class CCPM_FormComment
    {
        public Guid FormCommentId { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public Guid UserId { get; set; }

        public Guid? UserHistoryId { get; set; }

        public string DepartmentId { get; set; }

        public Guid FormId { get; set; }

        public string Content { get; set; }

        public virtual CCPM_Form Form { get; set; }
    }
}
