using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microservice.Calendar.Application.Models;
using Microservice.Calendar.Application.SharedQueries;
using Thunder.Platform.Cqrs;

namespace Microservice.Calendar.Application.Queries
{
    public class GetTeamMemberEventsQueryHandler : BaseThunderQueryHandler<GetTeamMemberEventsQuery, List<TeamMemberEventModel>>
    {
        private readonly TeamMemberEventOverviewSharedQuery _teamMemberEventSharedQuery;

        public GetTeamMemberEventsQueryHandler(
            TeamMemberEventOverviewSharedQuery teamMemberEventSharedQuery)
        {
            _teamMemberEventSharedQuery = teamMemberEventSharedQuery;
        }

        protected override Task<List<TeamMemberEventModel>> HandleAsync(GetTeamMemberEventsQuery query, CancellationToken cancellationToken)
        {
            var rangeStart = query.Request.RangeStart.Value;
            var rangeEnd = query.Request.RangeEnd.Value;

            return _teamMemberEventSharedQuery
                .GetTeamMemberEvents(query.Request.LearnerId, query.ApproveOfficerId, rangeStart, rangeEnd, cancellationToken);
        }
    }
}
