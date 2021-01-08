using System;
using System.Collections.Generic;

namespace Microservice.Analytics.Domain.Entities
{
    public partial class MT_Category
    {
        public MT_Category()
        {
            CamCourseCategory = new HashSet<CAM_CourseCategory>();
        }

        public Guid CategoryId { get; set; }

        public string FullStatement { get; set; }

        public string DisplayText { get; set; }

        public string GroupCode { get; set; }

        public string CodingScheme { get; set; }

        public string Note { get; set; }

        public string Type { get; set; }

        public virtual ICollection<CAM_CourseCategory> CamCourseCategory { get; set; }
    }
}
