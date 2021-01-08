using System;

namespace Microservice.Analytics.Domain.Entities
{
    public partial class CAM_CourseCoCurricularActivity
    {
        public Guid CourseId { get; set; }

        public Guid CoCurricularActivityId { get; set; }

        public DateTime CreatedDate { get; set; }

        public virtual MT_CoCurricularActivity CoCurricularActivity { get; set; }

        public virtual CAM_Course Course { get; set; }
    }
}
