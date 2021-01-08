using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microservice.Calendar.Application.Models;
using Microservice.Calendar.Application.SharedQueries;

namespace Microservice.Calendar.Application.Queries
{
    public class GetSharedTeamMemberEventOverviewQueryHandler
        : BaseQueryHandler<GetSharedTeamMemberEventOverviewQuery, List<TeamMemberEventOverviewModel>>
    {
        private readonly TeamMemberEventOverviewSharedQuery _teamMemberEventSharedQuery;

        public GetSharedTeamMemberEventOverviewQueryHandler(
            TeamMemberEventOverviewSharedQuery teamMemberEventSharedQuery)
        {
            _teamMemberEventSharedQuery = teamMemberEventSharedQuery;
        }

        protected override async Task<List<TeamMemberEventOverviewModel>> HandleAsync(
            GetSharedTeamMemberEventOverviewQuery query,
            CancellationToken cancellationToken)
        {
            var accessShareOwnerId = await _teamMemberEventSharedQuery
                .GetOwnerIdOfUserByAccessSharingId(query.AccessShareId, query.CurrentUserId);

            return await _teamMemberEventSharedQuery
                .GetTeamMemberEventOverview(accessShareOwnerId, query.RangeStart, query.RangeEnd, cancellationToken);
        }
    }
}
