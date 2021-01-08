using System;
using Thunder.Platform.Core.Domain.Auditing;

namespace Microservice.Analytics.Domain.Entities
{
    public partial class Learner_UserBookmarksDigitalContent : AuditedEntity
    {
        public Guid UserId { get; set; }

        public string UserHistoryId { get; set; }

        public string DepartmentId { get; set; }

        public Guid DigitalContentId { get; set; }

        public string Comment { get; set; }

        public Guid CreatedBy { get; set; }

        public Guid? ChangedBy { get; set; }

        public DateTime? FromDate { get; set; }

        public DateTime? ToDate { get; set; }

        public virtual SAM_Department Department { get; set; }
    }
}
