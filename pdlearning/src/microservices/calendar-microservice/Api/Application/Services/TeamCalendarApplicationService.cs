using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microservice.Calendar.Application.Models;
using Microservice.Calendar.Application.Queries;
using Microservice.Calendar.Application.RequestDtos;
using Thunder.Platform.Core.Application;
using Thunder.Platform.Cqrs;

namespace Microservice.Calendar.Application.Services
{
    public class TeamCalendarApplicationService : ApplicationService, ITeamCalendarApplicationService
    {
        private readonly IThunderCqrs _thunderCqrs;

        public TeamCalendarApplicationService(IThunderCqrs thunderCqrs)
        {
            _thunderCqrs = thunderCqrs;
        }

        public Task<List<TeamMemberEventOverviewModel>> GetTeamMemberEventOverview(Guid approveOfficerId, GetTeamMemberEventOverviewRequest request)
        {
            var query = new GetTeamMemberEventOverviewQuery
            {
                ApproveOfficerId = approveOfficerId,
                RangeStart = request.RangeStart.Value,
                RangeEnd = request.RangeEnd.Value
            };

            return _thunderCqrs.SendQuery(query);
        }

        public Task<List<TeamMemberEventModel>> GetTeamMemberEvents(
            GetTeamMemberEventsRequest request,
            Guid approveOfficerId)
        {
            var query = new GetTeamMemberEventsQuery
            {
                Request = request,
                ApproveOfficerId = approveOfficerId
            };

            return _thunderCqrs.SendQuery(query);
        }

        public Task<List<TeamMemberModel>> GetTeamMembers(Guid approveOfficerId)
        {
            var query = new GetTeamMembersQuery
            {
                ApproveOfficerId = approveOfficerId
            };

            return _thunderCqrs.SendQuery(query);
        }
    }
}
