using System;

namespace Microservice.Analytics.Domain.Entities
{
    public partial class CAM_CourseLearningArea
    {
        public Guid CourseId { get; set; }

        public Guid LearningAreaId { get; set; }

        public DateTime CreatedDate { get; set; }

        public virtual CAM_Course Course { get; set; }

        public virtual MT_LearningArea LearningArea { get; set; }
    }
}
