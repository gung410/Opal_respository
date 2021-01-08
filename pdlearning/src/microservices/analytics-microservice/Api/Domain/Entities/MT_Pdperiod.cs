using System;
using System.Collections.Generic;

namespace Microservice.Analytics.Domain.Entities
{
    public partial class MT_Pdperiod
    {
        public MT_Pdperiod()
        {
            CamCoursePdperiod = new HashSet<CAM_CoursePdperiod>();
        }

        public Guid PdperiodId { get; set; }

        public string FullStatement { get; set; }

        public string DisplayText { get; set; }

        public string GroupCode { get; set; }

        public string CodingScheme { get; set; }

        public virtual ICollection<CAM_CoursePdperiod> CamCoursePdperiod { get; set; }
    }
}
