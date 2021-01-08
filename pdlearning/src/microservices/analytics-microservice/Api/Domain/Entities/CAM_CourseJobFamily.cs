using System;

namespace Microservice.Analytics.Domain.Entities
{
    public partial class CAM_CourseJobFamily
    {
        public Guid CourseId { get; set; }

        public Guid JobFamilyId { get; set; }

        public DateTime CreatedDate { get; set; }

        public virtual CAM_Course Course { get; set; }

        public virtual MT_JobFamily JobFamily { get; set; }
    }
}
