using System;
using System.Collections.Generic;

namespace Microservice.Analytics.Domain.Entities
{
    public partial class MT_CoCurricularActivity
    {
        public MT_CoCurricularActivity()
        {
            CamCourseCoCurricularActivity = new HashSet<CAM_CourseCoCurricularActivity>();
            SamUserCoCurricularActivity = new HashSet<SAM_UserCoCurricularActivity>();
        }

        public Guid CoCurricularActivityId { get; set; }

        public string FullStatement { get; set; }

        public string DisplayText { get; set; }

        public string GroupCode { get; set; }

        public string CodingScheme { get; set; }

        public string Note { get; set; }

        public string Type { get; set; }

        public virtual ICollection<CAM_CourseCoCurricularActivity> CamCourseCoCurricularActivity { get; set; }

        public virtual ICollection<SAM_UserCoCurricularActivity> SamUserCoCurricularActivity { get; set; }
    }
}
