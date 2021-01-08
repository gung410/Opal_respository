using Microservice.Learner.Application.Models;
using Microservice.Learner.Domain.ValueObject;
using Thunder.Platform.Core.Application.Dtos;
using Thunder.Platform.Cqrs;

namespace Microservice.Learner.Application.Queries
{
    public class GetUserSharingQuery : BaseThunderQuery<PagedResultDto<UserSharingModel>>, IPagedResultAware
    {
        public string SearchText { get; set; }

        public SharingType ItemType { get; set; }

        public PagedResultRequestDto PageInfo { get; set; }
    }
}
