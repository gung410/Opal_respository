using Microservice.LnaForm.Application.Models;
using Microservice.LnaForm.Application.RequestDtos;
using Microservice.LnaForm.Application.Services;
using Thunder.Platform.Core.Application.Dtos;
using Thunder.Platform.Cqrs;

namespace Microservice.LnaForm.Application.Queries
{
    public class SearchAccessRightQuery : BaseThunderQuery<PagedResultDto<AccessRightModel>>
    {
        public SearchAccessRightRequest Request { get; set; }
    }
}
