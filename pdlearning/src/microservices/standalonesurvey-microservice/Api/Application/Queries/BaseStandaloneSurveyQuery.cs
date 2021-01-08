using System;
using Thunder.Platform.Cqrs;

namespace Microservice.StandaloneSurvey.Application.Queries
{
    public class BaseStandaloneSurveyQuery<T> : BaseThunderQuery<T>, IHasSubModuleInfo
    {
        private SubModule _subModule;

        public SubModule SubModule
        {
            get => _subModule;
            set => _subModule = value;
        }

        public Guid? CommunityId { get; set; }
    }
}
