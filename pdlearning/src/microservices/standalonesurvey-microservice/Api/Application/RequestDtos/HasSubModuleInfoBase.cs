using System;

namespace Microservice.StandaloneSurvey.Application.RequestDtos
{
    public class HasSubModuleInfoBase : IHasSubModuleInfo
    {
        public SubModule SubModule { get; set; }

        public Guid? CommunityId { get; set; }
    }
}
