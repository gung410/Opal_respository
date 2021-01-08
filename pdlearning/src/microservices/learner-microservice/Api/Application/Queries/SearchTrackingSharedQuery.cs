using Microservice.Learner.Application.Models;
using Thunder.Platform.Core.Application.Dtos;
using Thunder.Platform.Cqrs;

namespace Microservice.Learner.Application.Queries
{
    public class SearchTrackingSharedQuery : BaseThunderQuery<PagedResultDto<TrackingSharedDetailByModel>>, IPagedResultAware
    {
        public PagedResultRequestDto PageInfo { get; set; }
    }
}
