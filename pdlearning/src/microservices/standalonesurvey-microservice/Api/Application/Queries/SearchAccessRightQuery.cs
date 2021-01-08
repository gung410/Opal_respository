using Microservice.StandaloneSurvey.Application.Models;
using Microservice.StandaloneSurvey.Application.RequestDtos;
using Thunder.Platform.Core.Application.Dtos;
using Thunder.Platform.Cqrs;

namespace Microservice.StandaloneSurvey.Application.Queries
{
    public class SearchAccessRightQuery : BaseStandaloneSurveyQuery<PagedResultDto<AccessRightModel>>
    {
        public SearchAccessRightRequest Request { get; set; }
    }
}
