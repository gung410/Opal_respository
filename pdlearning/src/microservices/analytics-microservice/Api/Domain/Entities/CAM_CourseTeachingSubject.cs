using System;

namespace Microservice.Analytics.Domain.Entities
{
    public partial class CAM_CourseTeachingSubject
    {
        public Guid CourseId { get; set; }

        public Guid TeachingSubjectId { get; set; }

        public DateTime CreatedDate { get; set; }

        public virtual CAM_Course Course { get; set; }

        public virtual MT_TeachingSubject TeachingSubject { get; set; }
    }
}
