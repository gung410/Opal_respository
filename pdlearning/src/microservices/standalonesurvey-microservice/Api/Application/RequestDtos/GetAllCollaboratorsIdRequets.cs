using System;

namespace Microservice.StandaloneSurvey.Application.RequestDtos
{
    public class GetAllAccessRightIdsRequest : HasSubModuleInfoBase
    {
        public Guid OriginalObjectId { get; set; }
    }
}
