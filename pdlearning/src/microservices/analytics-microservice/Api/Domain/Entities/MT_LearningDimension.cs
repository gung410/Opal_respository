using System;
using System.Collections.Generic;

namespace Microservice.Analytics.Domain.Entities
{
    public partial class MT_LearningDimension
    {
        public MT_LearningDimension()
        {
            CamCourseLearningDimension = new HashSet<CAM_CourseLearningDimension>();
            MtLearningArea = new HashSet<MT_LearningArea>();
        }

        public Guid LearningDimensionId { get; set; }

        public Guid LearningFrameworkId { get; set; }

        public string FullStatement { get; set; }

        public string DisplayText { get; set; }

        public string GroupCode { get; set; }

        public string CodingScheme { get; set; }

        public string Note { get; set; }

        public string Type { get; set; }

        public virtual MT_LearningFramework LearningFramework { get; set; }

        public virtual ICollection<CAM_CourseLearningDimension> CamCourseLearningDimension { get; set; }

        public virtual ICollection<MT_LearningArea> MtLearningArea { get; set; }
    }
}
