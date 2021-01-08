using System;

namespace Microservice.Analytics.Domain.Entities
{
    public partial class CAM_CoursePdperiod
    {
        public Guid CourseId { get; set; }

        public Guid PdperiodId { get; set; }

        public DateTime CreatedDate { get; set; }

        public virtual CAM_Course Course { get; set; }

        public virtual MT_Pdperiod Pdperiod { get; set; }
    }
}
