using System;
using System.Collections.Generic;

namespace Microservice.Analytics.Domain.Entities
{
    public partial class MT_SubjectGroup
    {
        public MT_SubjectGroup()
        {
            MtSubjectKeyWords = new HashSet<MT_SubjectKeyWords>();
        }

        public Guid SubjectGroupId { get; set; }

        public Guid? SubjectId { get; set; }

        public string FullStatement { get; set; }

        public string DisplayText { get; set; }

        public virtual MT_Subject Subject { get; set; }

        public virtual ICollection<MT_SubjectKeyWords> MtSubjectKeyWords { get; set; }
    }
}
