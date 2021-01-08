using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microservice.Calendar.Application.Models;
using Microservice.Calendar.Domain.Entities;
using Microservice.Calendar.Domain.Extensions;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Domain.Repositories;

namespace Microservice.Calendar.Application.Queries
{
    public class GetMySharedTeamsQueryHandler : BaseQueryHandler<GetMySharedTeamsQuery, List<SharedTeamModel>>
    {
        private readonly IRepository<TeamAccessSharing> _teamAccessSharingRepo;
        private readonly IRepository<CalendarUser> _userRepo;

        public GetMySharedTeamsQueryHandler(
            IRepository<TeamAccessSharing> teamAccessSharingRepo,
            IRepository<CalendarUser> userRepo)
        {
            _teamAccessSharingRepo = teamAccessSharingRepo;
            _userRepo = userRepo;
        }

        protected override Task<List<SharedTeamModel>> HandleAsync(GetMySharedTeamsQuery query, CancellationToken cancellationToken)
        {
            return _teamAccessSharingRepo
                .GetSharedTeams(query.UserId, _userRepo)
                .ToListAsync(cancellationToken);
        }
    }
}
