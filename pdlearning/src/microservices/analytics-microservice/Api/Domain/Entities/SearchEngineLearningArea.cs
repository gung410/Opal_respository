using System;

namespace Microservice.Analytics.Domain.Entities
{
    public partial class SearchEngineLearningArea
    {
        public Guid SearchEngineId { get; set; }

        public Guid LearningAreaId { get; set; }

        public virtual SearchEngine SearchEngine { get; set; }
    }
}
