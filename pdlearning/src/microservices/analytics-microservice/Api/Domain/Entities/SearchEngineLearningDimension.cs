using System;

namespace Microservice.Analytics.Domain.Entities
{
    public partial class SearchEngineLearningDimension
    {
        public Guid SearchEngineId { get; set; }

        public Guid LearningDimensionId { get; set; }

        public virtual SearchEngine SearchEngine { get; set; }
    }
}
