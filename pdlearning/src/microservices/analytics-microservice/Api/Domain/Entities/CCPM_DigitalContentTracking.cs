using System;
using Thunder.Platform.Core.Domain.Auditing;

namespace Microservice.Analytics.Domain.Entities
{
    public class CCPM_DigitalContentTracking : AuditedEntity
    {
        public Guid UserId { get; set; }

        public Guid? UserHistoryId { get; set; }

        public string DepartmentId { get; set; }

        public Guid DigitalContentId { get; set; }

        public Guid? CourseId { get; set; }

        public Guid? ClassRunId { get; set; }

        public string Action { get; set; }
    }
}
