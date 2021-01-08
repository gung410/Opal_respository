using System;
using System.Collections.Generic;

namespace Microservice.Analytics.Domain.Entities
{
    public partial class MT_TeachingLevel
    {
        public MT_TeachingLevel()
        {
            CamCourseTeachingLevel = new HashSet<CAM_CourseTeachingLevel>();
            SamUserTeachingLevel = new HashSet<SAM_UserTeachingLevel>();
        }

        public Guid TeachingLevelId { get; set; }

        public string FullStatement { get; set; }

        public string DisplayText { get; set; }

        public string GroupCode { get; set; }

        public string CodingScheme { get; set; }

        public string Note { get; set; }

        public string Type { get; set; }

        public virtual ICollection<CAM_CourseTeachingLevel> CamCourseTeachingLevel { get; set; }

        public virtual ICollection<SAM_UserTeachingLevel> SamUserTeachingLevel { get; set; }
    }
}
