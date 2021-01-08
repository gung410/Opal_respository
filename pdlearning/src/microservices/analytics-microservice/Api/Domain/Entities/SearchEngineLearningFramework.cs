using System;

namespace Microservice.Analytics.Domain.Entities
{
    public partial class SearchEngineLearningFramework
    {
        public Guid SearchEngineId { get; set; }

        public Guid LearningFrameworkId { get; set; }

        public virtual SearchEngine SearchEngine { get; set; }
    }
}
