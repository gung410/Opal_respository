using System;
using System.Collections.Generic;

namespace Microservice.Analytics.Domain.Entities
{
    public partial class MT_LearningFramework
    {
        public MT_LearningFramework()
        {
            CamCourseEasSubstantiveGradeBanding = new HashSet<CAM_CourseEasSubstantiveGradeBanding>();
            CamCourseLearningFramework = new HashSet<CAM_CourseLearningFramework>();
            MtLearningDimension = new HashSet<MT_LearningDimension>();
        }

        public Guid LearningFrameworkId { get; set; }

        public Guid? ServiceSchemeId { get; set; }

        public string FullStatement { get; set; }

        public string DisplayText { get; set; }

        public string GroupCode { get; set; }

        public string CodingScheme { get; set; }

        public string Note { get; set; }

        public string Type { get; set; }

        public virtual MT_ServiceScheme ServiceScheme { get; set; }

        public virtual ICollection<CAM_CourseEasSubstantiveGradeBanding> CamCourseEasSubstantiveGradeBanding { get; set; }

        public virtual ICollection<CAM_CourseLearningFramework> CamCourseLearningFramework { get; set; }

        public virtual ICollection<MT_LearningDimension> MtLearningDimension { get; set; }
    }
}
