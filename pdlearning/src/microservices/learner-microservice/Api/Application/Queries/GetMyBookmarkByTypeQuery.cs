using Microservice.Learner.Application.Models;
using Microservice.Learner.Domain.ValueObject;
using Thunder.Platform.Core.Application.Dtos;
using Thunder.Platform.Cqrs;

namespace Microservice.Learner.Application.Queries
{
    public class GetMyBookmarkByTypeQuery : BaseThunderQuery<PagedResultDto<UserBookmarkModel>>, IPagedResultAware
    {
        public BookmarkType ItemType { get; set; }

        public PagedResultRequestDto PageInfo { get; set; }
    }
}
