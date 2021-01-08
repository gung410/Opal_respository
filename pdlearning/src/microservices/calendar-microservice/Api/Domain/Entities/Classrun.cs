using System;
using Microservice.Calendar.Domain.Enums;
using Thunder.Platform.Core.Domain.Auditing;

namespace Microservice.Calendar.Domain.Entities
{
    // The ClassRun is not belongs to the calendar domain.
    // It was synced from CAM module in order to track the relevant events.
    public class ClassRun : FullAuditedEntity
    {
        public Guid CourseId { get; set; }

        public ClassRunStatus Status { get; set; }
    }
}
