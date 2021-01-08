using System;

namespace Microservice.Analytics.Domain.Entities
{
    public partial class CAM_CoursePreRequisite
    {
        public Guid CourseId { get; set; }

        public Guid PreRequisiteCourseId { get; set; }

        public DateTime CreatedDate { get; set; }

        public virtual CAM_Course Course { get; set; }

        public virtual CAM_Course PreRequisiteCourse { get; set; }
    }
}
