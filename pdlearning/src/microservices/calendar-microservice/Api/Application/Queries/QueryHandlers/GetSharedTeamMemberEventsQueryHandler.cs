using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microservice.Calendar.Application.Models;
using Microservice.Calendar.Application.SharedQueries;

namespace Microservice.Calendar.Application.Queries.QueryHandlers
{
    public class GetSharedTeamMemberEventsQueryHandler : BaseQueryHandler<GetSharedTeamMemberEventsQuery, List<TeamMemberEventModel>>
    {
        private readonly TeamMemberEventOverviewSharedQuery _teamMemberEventSharedQuery;

        public GetSharedTeamMemberEventsQueryHandler(
            TeamMemberEventOverviewSharedQuery teamMemberEventSharedQuery)
        {
            _teamMemberEventSharedQuery = teamMemberEventSharedQuery;
        }

        protected override async Task<List<TeamMemberEventModel>> HandleAsync(GetSharedTeamMemberEventsQuery query, CancellationToken cancellationToken)
        {
            var rangeStart = query.Request.RangeStart.Value;
            var rangeEnd = query.Request.RangeEnd.Value;

            var accessShareOwnerId = await _teamMemberEventSharedQuery
                .GetOwnerIdOfUserByAccessSharingId(query.Request.AccessShareId, query.SharedWithUserId);

            return await _teamMemberEventSharedQuery
                .GetTeamMemberEvents(query.Request.LearnerId, accessShareOwnerId, rangeStart, rangeEnd, cancellationToken);
        }
    }
}
