using System;

namespace Microservice.Analytics.Domain.Entities
{
    public partial class SearchEngineServiceScheme
    {
        public Guid SearchEngineId { get; set; }

        public Guid ServiceSchemeId { get; set; }

        public virtual SearchEngine SearchEngine { get; set; }
    }
}
