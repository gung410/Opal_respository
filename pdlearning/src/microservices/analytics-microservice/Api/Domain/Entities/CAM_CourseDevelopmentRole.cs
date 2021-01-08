using System;

namespace Microservice.Analytics.Domain.Entities
{
    public partial class CAM_CourseDevelopmentRole
    {
        public Guid CourseId { get; set; }

        public Guid DevelopmentRoleId { get; set; }

        public DateTime CreatedDate { get; set; }

        public virtual CAM_Course Course { get; set; }

        public virtual MT_DevelopmentRole DevelopmentRole { get; set; }
    }
}
