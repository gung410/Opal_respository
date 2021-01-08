using System;

namespace Microservice.StandaloneSurvey
{
    public interface IHasSubModuleInfo
    {
        SubModule SubModule { get; set; }

        Guid? CommunityId { get; set; }
    }
}
