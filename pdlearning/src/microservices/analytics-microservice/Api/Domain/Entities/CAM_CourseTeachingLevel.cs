using System;

namespace Microservice.Analytics.Domain.Entities
{
    public partial class CAM_CourseTeachingLevel
    {
        public Guid CourseId { get; set; }

        public Guid TeachingLevelId { get; set; }

        public DateTime CreatedDate { get; set; }

        public virtual CAM_Course Course { get; set; }

        public virtual MT_TeachingLevel TeachingLevel { get; set; }
    }
}
