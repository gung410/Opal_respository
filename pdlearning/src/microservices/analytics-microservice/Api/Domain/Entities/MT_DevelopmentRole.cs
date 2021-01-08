using System;
using System.Collections.Generic;

namespace Microservice.Analytics.Domain.Entities
{
    public partial class MT_DevelopmentRole
    {
        public MT_DevelopmentRole()
        {
            CamCourseDevelopmentRole = new HashSet<CAM_CourseDevelopmentRole>();
        }

        public Guid DevelopmentRoleId { get; set; }

        public Guid? ServiceSchemeId { get; set; }

        public string FullStatement { get; set; }

        public string DisplayText { get; set; }

        public string GroupCode { get; set; }

        public string CodingScheme { get; set; }

        public virtual MT_ServiceScheme ServiceScheme { get; set; }

        public virtual ICollection<CAM_CourseDevelopmentRole> CamCourseDevelopmentRole { get; set; }
    }
}
