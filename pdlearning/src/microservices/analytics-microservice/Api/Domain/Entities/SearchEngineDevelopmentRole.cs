using System;

namespace Microservice.Analytics.Domain.Entities
{
    public partial class SearchEngineDevelopmentRole
    {
        public Guid SearchEngineId { get; set; }

        public Guid DevelopmentRoleId { get; set; }

        public virtual SearchEngine SearchEngine { get; set; }
    }
}
