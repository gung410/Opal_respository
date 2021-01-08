using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microservice.Calendar.Application.Models;
using Microservice.Calendar.Application.SharedQueries;
using Thunder.Platform.Cqrs;

namespace Microservice.Calendar.Application.Queries.QueryHandlers
{
    public class GetTeamMemberEventOverviewQueryHandler : BaseThunderQueryHandler<GetTeamMemberEventOverviewQuery, List<TeamMemberEventOverviewModel>>
    {
        private readonly TeamMemberEventOverviewSharedQuery _teamMemberEventSharedQuery;

        public GetTeamMemberEventOverviewQueryHandler(
            TeamMemberEventOverviewSharedQuery teamMemberEventSharedQuery)
        {
            _teamMemberEventSharedQuery = teamMemberEventSharedQuery;
        }

        protected override async Task<List<TeamMemberEventOverviewModel>> HandleAsync(GetTeamMemberEventOverviewQuery query, CancellationToken cancellationToken)
        {
            return await _teamMemberEventSharedQuery
                .GetTeamMemberEventOverview(query.ApproveOfficerId, query.RangeStart, query.RangeEnd, cancellationToken);
        }
    }
}
