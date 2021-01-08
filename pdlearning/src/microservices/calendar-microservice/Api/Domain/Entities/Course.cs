using Microservice.Calendar.Domain.Enums;
using Thunder.Platform.Core.Domain.Auditing;

namespace Microservice.Calendar.Domain.Entities
{
    // The Course entity is not belongs to the calendar domain.
    // It was synced from CAM module in order to track the relevant events from Course.
    public class Course : FullAuditedEntity
    {
        public string CourseName { get; set; }

        public CourseStatus Status { get; set; }
    }
}
