using System;
using System.Collections.Generic;

namespace Microservice.Analytics.Domain.Entities
{
    public partial class MT_TeachingSubject
    {
        public MT_TeachingSubject()
        {
            CamCourseTeachingSubject = new HashSet<CAM_CourseTeachingSubject>();
            SamUserTeachingSubject = new HashSet<SAM_UserTeachingSubject>();
        }

        public Guid TeachingSubjectId { get; set; }

        public string FullStatement { get; set; }

        public string DisplayText { get; set; }

        public string GroupCode { get; set; }

        public string CodingScheme { get; set; }

        public string Note { get; set; }

        public string Type { get; set; }

        public virtual ICollection<CAM_CourseTeachingSubject> CamCourseTeachingSubject { get; set; }

        public virtual ICollection<SAM_UserTeachingSubject> SamUserTeachingSubject { get; set; }
    }
}
