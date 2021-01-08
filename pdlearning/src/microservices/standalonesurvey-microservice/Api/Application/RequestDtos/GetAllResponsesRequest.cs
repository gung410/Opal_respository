using Thunder.Platform.Core.Application.Dtos;

namespace Microservice.StandaloneSurvey.Application.RequestDtos
{
    public class GetAllResponsesRequest : HasSubModuleInfoBase
    {
        public PagedResultRequestDto PagedInfo { get; set; }
    }
}
