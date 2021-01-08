using System;

namespace Microservice.Analytics.Domain.Entities
{
    public partial class CAM_CourseLearningSubArea
    {
        public Guid CourseId { get; set; }

        public Guid LearningSubAreaId { get; set; }

        public DateTime CreatedDate { get; set; }

        public virtual CAM_Course Course { get; set; }

        public virtual MT_LearningSubArea LearningSubArea { get; set; }
    }
}
