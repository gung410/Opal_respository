using System;

namespace Microservice.Analytics.Domain.Entities
{
    public partial class CAM_CourseLearningDimension
    {
        public Guid CourseId { get; set; }

        public Guid LearningDimensionId { get; set; }

        public DateTime CreatedDate { get; set; }

        public virtual CAM_Course Course { get; set; }

        public virtual MT_LearningDimension LearningDimension { get; set; }
    }
}
