using Microservice.Content.Application.Models;
using Thunder.Platform.Core.Application.Dtos;
using Thunder.Platform.Cqrs;

namespace Microservice.Content.Application.Queries
{
    public class GetPendingApprovalDigitalContentsQuery : BaseThunderQuery<PagedResultDto<SearchDigitalContentModel>>
    {
        public PagedResultRequestDto PagedInfo { get; set; }
    }
}
