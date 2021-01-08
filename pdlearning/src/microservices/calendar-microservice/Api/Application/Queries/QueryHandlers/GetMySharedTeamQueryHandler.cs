using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microservice.Calendar.Application.Models;
using Microservice.Calendar.Application.SharedQueries;

namespace Microservice.Calendar.Application.Queries.QueryHandlers
{
    public class GetMySharedTeamQueryHandler : BaseQueryHandler<GetSharedTeamMembersQuery, List<TeamMemberModel>>
    {
        private readonly TeamMemberEventOverviewSharedQuery _teamMemberEventSharedQuery;

        public GetMySharedTeamQueryHandler(
            TeamMemberEventOverviewSharedQuery teamMemberEventSharedQuery)
        {
            _teamMemberEventSharedQuery = teamMemberEventSharedQuery;
        }

        protected override async Task<List<TeamMemberModel>> HandleAsync(GetSharedTeamMembersQuery query, CancellationToken cancellationToken)
        {
            var accessShareOwnerId = await _teamMemberEventSharedQuery
                .GetOwnerIdOfUserByAccessSharingId(query.AccessShareId, query.UserId);

            return await _teamMemberEventSharedQuery.GetTeamMember(accessShareOwnerId, cancellationToken);
        }
    }
}
