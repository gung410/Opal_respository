using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microservice.Calendar.Application.Commands;
using Microservice.Calendar.Application.Models;
using Microservice.Calendar.Application.Queries;
using Microservice.Calendar.Application.RequestDtos;
using Thunder.Platform.Core.Application;
using Thunder.Platform.Core.Application.Dtos;
using Thunder.Platform.Cqrs;

namespace Microservice.Calendar.Application.Services
{
    public class TeamCalendarSharingApplicationService : ApplicationService, ITeamCalendarSharingApplicationService
    {
        private readonly IThunderCqrs _thunderCqrs;

        public TeamCalendarSharingApplicationService(
            IThunderCqrs thunderCqrs)
        {
            _thunderCqrs = thunderCqrs;
        }

        public Task<List<SharedTeamModel>> GetMySharedTeams(Guid userId)
        {
            var query = new GetMySharedTeamsQuery
            {
                UserId = userId
            };

            return _thunderCqrs.SendQuery(query);
        }

        public Task<List<TeamMemberEventOverviewModel>> GetSharedTeamMemberEventOverview(
            GetSharedTeamMemberEventOverviewRequest request,
            Guid currentUserId)
        {
            var query = new GetSharedTeamMemberEventOverviewQuery
            {
                CurrentUserId = currentUserId,
                AccessShareId = request.AccessShareId.Value,
                RangeStart = request.RangeStart.Value,
                RangeEnd = request.RangeEnd.Value
            };

            return _thunderCqrs.SendQuery(query);
        }

        public Task<List<TeamMemberModel>> GetSharedTeamMembers(Guid accessShareId, Guid userId)
        {
            var query = new GetSharedTeamMembersQuery
            {
                UserId = userId,
                AccessShareId = accessShareId
            };

            return _thunderCqrs.SendQuery(query);
        }

        public Task<List<TeamMemberEventModel>> GetSharedTeamMemberEvents(GetSharedTeamMemberEventsRequest request, Guid currentUserId)
        {
            var query = new GetSharedTeamMemberEventsQuery
            {
                SharedWithUserId = currentUserId,
                Request = request
            };

            return _thunderCqrs.SendQuery(query);
        }

        public Task SaveCalendarAccessSharings(SaveTeamCalendarAccessSharingsRequest request, Guid ownerId)
        {
            var command = new SaveTeamCalendarAccessSharingsCommand
            {
                OwnerId = ownerId,
                Request = request
            };

            return _thunderCqrs.SendCommand(command);
        }

        public Task<PagedResultDto<UserAccessSharingModel>> GetCalendarAccessSharings(Guid ownerId, PagedResultRequestDto request)
        {
            var query = new GetCalendarAccessSharingsQuery
            {
                OwnerId = ownerId,
                PagingRequest = request
            };

            return _thunderCqrs.SendQuery(query);
        }
    }
}
