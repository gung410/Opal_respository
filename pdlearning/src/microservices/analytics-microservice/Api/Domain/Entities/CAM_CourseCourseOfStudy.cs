using System;

namespace Microservice.Analytics.Domain.Entities
{
    public partial class CAM_CourseCourseOfStudy
    {
        public Guid CourseId { get; set; }

        public Guid CourseOfStudyId { get; set; }

        public DateTime CreatedDate { get; set; }

        public virtual CAM_Course Course { get; set; }

        public virtual MT_CourseOfStudy CourseOfStudy { get; set; }
    }
}
