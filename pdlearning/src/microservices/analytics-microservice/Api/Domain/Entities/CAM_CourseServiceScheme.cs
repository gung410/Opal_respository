using System;

namespace Microservice.Analytics.Domain.Entities
{
    public partial class CAM_CourseServiceScheme
    {
        public Guid CourseId { get; set; }

        public Guid ServiceSchemeId { get; set; }

        public DateTime CreatedDate { get; set; }

        public virtual CAM_Course Course { get; set; }

        public virtual MT_ServiceScheme ServiceScheme { get; set; }
    }
}
