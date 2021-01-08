using System.Threading;
using System.Threading.Tasks;
using Microservice.Calendar.Application.Models;
using Microservice.Calendar.Domain.Entities;
using Microservice.Calendar.Domain.Extensions;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Application.Dtos;
using Thunder.Platform.Core.Domain.Repositories;

namespace Microservice.Calendar.Application.Queries
{
    public class GetCalendarAccessSharingsQueryHandler : BaseQueryHandler<GetCalendarAccessSharingsQuery, PagedResultDto<UserAccessSharingModel>>
    {
        private readonly IRepository<TeamAccessSharing> _teamAccessSharingRepo;
        private readonly IRepository<CalendarUser> _userRepo;

        public GetCalendarAccessSharingsQueryHandler(
            IRepository<TeamAccessSharing> teamAccessSharingRepo,
            IRepository<CalendarUser> userRepo)
        {
            _teamAccessSharingRepo = teamAccessSharingRepo;
            _userRepo = userRepo;
        }

        protected override async Task<PagedResultDto<UserAccessSharingModel>> HandleAsync(GetCalendarAccessSharingsQuery query, CancellationToken cancellationToken)
        {
            var userAccessSharings = _teamAccessSharingRepo.GetUserAccessSharings(query.OwnerId, _userRepo);
            var totalCount = await userAccessSharings.CountAsync(cancellationToken);
            var pagedResults = await ApplyPaging(userAccessSharings, query.PagingRequest).ToListAsync(cancellationToken);

            return new PagedResultDto<UserAccessSharingModel>(totalCount, pagedResults);
        }
    }
}
