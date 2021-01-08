using System;
using System.Collections.Generic;

namespace Microservice.Analytics.Domain.Entities
{
    public partial class MT_ServiceScheme
    {
        public MT_ServiceScheme()
        {
            CamCourseServiceScheme = new HashSet<CAM_CourseServiceScheme>();
            MtDevelopmentRole = new HashSet<MT_DevelopmentRole>();
            MtLearningFramework = new HashSet<MT_LearningFramework>();
            MtSubject = new HashSet<MT_Subject>();
        }

        public Guid ServiceSchemeId { get; set; }

        public string FullStatement { get; set; }

        public string DisplayText { get; set; }

        public string GroupCode { get; set; }

        public string CodingScheme { get; set; }

        public string Note { get; set; }

        public string Type { get; set; }

        public virtual ICollection<CAM_CourseServiceScheme> CamCourseServiceScheme { get; set; }

        public virtual ICollection<MT_DevelopmentRole> MtDevelopmentRole { get; set; }

        public virtual ICollection<MT_LearningFramework> MtLearningFramework { get; set; }

        public virtual ICollection<MT_Subject> MtSubject { get; set; }
    }
}
