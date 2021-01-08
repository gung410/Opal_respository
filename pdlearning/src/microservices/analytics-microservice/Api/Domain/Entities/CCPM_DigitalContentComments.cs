using System;

namespace Microservice.Analytics.Domain.Entities
{
    public partial class CCPM_DigitalContentComments
    {
        public Guid DigitalContentCommentId { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public Guid UserId { get; set; }

        public Guid? UserHistoryId { get; set; }

        public string DepartmentId { get; set; }

        public Guid DigitalContentId { get; set; }

        public string Content { get; set; }

        public virtual CCPM_DigitalContent DigitalContent { get; set; }
    }
}
