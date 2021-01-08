using System;

namespace Microservice.Analytics.Domain.Entities
{
    public partial class SearchEngineTeachingLevel
    {
        public Guid SearchEngineId { get; set; }

        public Guid TeachingLevelId { get; set; }

        public virtual SearchEngine SearchEngine { get; set; }
    }
}
