using System;

namespace Microservice.Analytics.Domain.Entities
{
    public partial class CAM_CourseCategory
    {
        public Guid CourseId { get; set; }

        public Guid CategoryId { get; set; }

        public DateTime CreatedDate { get; set; }

        public virtual MT_Category Category { get; set; }

        public virtual CAM_Course Course { get; set; }
    }
}
