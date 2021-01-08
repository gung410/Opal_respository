using System;

namespace Microservice.Analytics.Domain.Entities
{
    public partial class SearchEngineLearningSubArea
    {
        public Guid SearchEngineId { get; set; }

        public Guid LearningSubAreaId { get; set; }

        public virtual SearchEngine SearchEngine { get; set; }
    }
}
