using System;

namespace Microservice.Analytics.Domain.Entities
{
    public partial class CAM_CourseEasSubstantiveGradeBanding
    {
        public Guid CourseId { get; set; }

        public Guid LearningFrameworkId { get; set; }

        public DateTime CreatedDate { get; set; }

        public virtual CAM_Course Course { get; set; }

        public virtual MT_LearningFramework LearningFramework { get; set; }
    }
}
