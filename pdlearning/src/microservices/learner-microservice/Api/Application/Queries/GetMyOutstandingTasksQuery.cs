using Microservice.Learner.Application.Models;
using Thunder.Platform.Core.Application.Dtos;
using Thunder.Platform.Cqrs;

namespace Microservice.Learner.Application.Queries
{
    public class GetMyOutstandingTasksQuery : BaseThunderQuery<PagedResultDto<OutstandingTaskModel>>, IPagedResultAware
    {
        public PagedResultRequestDto PageInfo { get; set; }
    }
}
