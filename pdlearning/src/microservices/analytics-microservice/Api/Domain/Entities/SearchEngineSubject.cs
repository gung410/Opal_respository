using System;

namespace Microservice.Analytics.Domain.Entities
{
    public partial class SearchEngineSubject
    {
        public Guid SearchEngineId { get; set; }

        public Guid SubjectId { get; set; }

        public virtual SearchEngine SearchEngine { get; set; }
    }
}
