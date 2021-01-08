using System;

namespace Microservice.Analytics.Domain.Entities
{
    public partial class SearchEngineCategory
    {
        public Guid SearchEngineId { get; set; }

        public Guid CategoryId { get; set; }

        public virtual SearchEngine SearchEngine { get; set; }
    }
}
