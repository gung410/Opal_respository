using System;
using Thunder.Platform.Cqrs;

namespace Microservice.StandaloneSurvey.Application.Commands
{
    public abstract class BaseStandaloneSurveyCommand : BaseThunderCommand, IHasSubModuleInfo
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
