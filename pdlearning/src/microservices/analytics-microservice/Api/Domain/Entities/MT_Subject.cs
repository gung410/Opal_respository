using System;
using System.Collections.Generic;

namespace Microservice.Analytics.Domain.Entities
{
    public partial class MT_Subject
    {
        public MT_Subject()
        {
            MtSubjectGroup = new HashSet<MT_SubjectGroup>();
            SamUserProfessionalInterestArea = new HashSet<SAM_UserProfessionalInterestArea>();
        }

        public Guid SubjectId { get; set; }

        public Guid? ServiceSchemeId { get; set; }

        public string FullStatement { get; set; }

        public string DisplayText { get; set; }

        public string GroupCode { get; set; }

        public string CodingScheme { get; set; }

        public string Note { get; set; }

        public string Type { get; set; }

        public virtual MT_ServiceScheme ServiceScheme { get; set; }

        public virtual ICollection<MT_SubjectGroup> MtSubjectGroup { get; set; }

        public virtual ICollection<SAM_UserProfessionalInterestArea> SamUserProfessionalInterestArea { get; set; }
    }
}
