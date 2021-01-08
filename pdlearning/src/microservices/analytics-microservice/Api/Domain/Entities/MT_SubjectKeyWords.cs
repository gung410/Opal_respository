using System;

namespace Microservice.Analytics.Domain.Entities
{
    public partial class MT_SubjectKeyWords
    {
        public Guid SubjectKeyWordId { get; set; }

        public Guid? SubjectGroupId { get; set; }

        public string FullStatement { get; set; }

        public string DisplayText { get; set; }

        public virtual MT_SubjectGroup SubjectGroup { get; set; }
    }
}
