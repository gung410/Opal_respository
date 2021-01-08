using System;

namespace Microservice.Analytics.Domain.Entities
{
    public partial class CAM_CourseApplicableZone
    {
        public Guid CourseId { get; set; }

        public string DepartmentId { get; set; }

        public DateTime CreatedDate { get; set; }

        public virtual CAM_Course Course { get; set; }

        public virtual SAM_Department Department { get; set; }
    }
}
