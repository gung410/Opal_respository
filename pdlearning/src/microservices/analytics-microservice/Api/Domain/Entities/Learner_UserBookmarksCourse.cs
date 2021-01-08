using System;
using Thunder.Platform.Core.Domain.Auditing;

namespace Microservice.Analytics.Domain.Entities
{
    public partial class Learner_UserBookmarksCourse : AuditedEntity
    {
        public Guid UserId { get; set; }

        public Guid UserHistoryId { get; set; }

        public string DepartmentId { get; set; }

        public Guid CourseId { get; set; }

        public string Comment { get; set; }

        public Guid CreatedBy { get; set; }

        public Guid? ChangedBy { get; set; }

        public string ItemType { get; set; }

        public DateTime? FromDate { get; set; }

        public DateTime? ToDate { get; set; }

        public virtual SAM_Department Department { get; set; }
    }
}
