using Microservice.Form.Application.Models;
using Microservice.Form.Application.RequestDtos;
using Microservice.Form.Application.Services;
using Thunder.Platform.Core.Application.Dtos;
using Thunder.Platform.Cqrs;

namespace Microservice.Form.Application.Queries
{
    public class SearchAccessRightQuery : BaseThunderQuery<PagedResultDto<AccessRightModel>>
    {
        public SearchAccessRightRequest Request { get; set; }
    }
}
