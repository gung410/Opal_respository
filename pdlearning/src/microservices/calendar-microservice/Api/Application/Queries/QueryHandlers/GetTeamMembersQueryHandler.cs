using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microservice.Calendar.Application.Models;
using Microservice.Calendar.Application.SharedQueries;
using Thunder.Platform.Cqrs;

namespace Microservice.Calendar.Application.Queries.QueryHandlers
{
    public class GetTeamMembersQueryHandler : BaseThunderQueryHandler<GetTeamMembersQuery, List<TeamMemberModel>>
    {
        private readonly TeamMemberEventOverviewSharedQuery _teamMemberEventSharedQuery;

        public GetTeamMembersQueryHandler(
            TeamMemberEventOverviewSharedQuery teamMemberEventSharedQuery)
        {
            _teamMemberEventSharedQuery = teamMemberEventSharedQuery;
        }

        protected override Task<List<TeamMemberModel>> HandleAsync(GetTeamMembersQuery query, CancellationToken cancellationToken)
        {
            return _teamMemberEventSharedQuery.GetTeamMember(query.ApproveOfficerId, cancellationToken);
        }
    }
}
