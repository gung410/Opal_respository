using Microservice.Learner.Application.Models;
using Thunder.Platform.Core.Application.Dtos;
using Thunder.Platform.Cqrs;

namespace Microservice.Learner.Application.Queries
{
    public class GetUsersQuery : BaseThunderQuery<PagedResultDto<UserModel>>, IPagedResultAware
    {
        public string SearchText { get; set; }

        public bool IncludeSubDepartments { get; set; }

        public PagedResultRequestDto PageInfo { get; set; }
    }
}
