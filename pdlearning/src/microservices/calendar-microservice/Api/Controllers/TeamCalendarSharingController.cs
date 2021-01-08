using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microservice.Calendar.Application.Models;
using Microservice.Calendar.Application.RequestDtos;
using Microservice.Calendar.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Thunder.Platform.AspNetCore.Controllers;
using Thunder.Platform.Core.Application.Dtos;
using Thunder.Platform.Core.Context;

namespace Microservice.Calendar.Controllers
{
    [Route("api/calendars/team/shares")]
    public class TeamCalendarSharingController : ApplicationApiController
    {
        private readonly ITeamCalendarSharingApplicationService _teamCalendarSharingService;

        public TeamCalendarSharingController(
            IUserContext userContext,
            ITeamCalendarSharingApplicationService teamCalendarSharingService)
            : base(userContext)
        {
            _teamCalendarSharingService = teamCalendarSharingService;
        }

        [HttpGet("mySharedTeams")]
        public Task<List<SharedTeamModel>> GetMySharedTeams()
        {
            return _teamCalendarSharingService.GetMySharedTeams(CurrentUserId);
        }

        [HttpGet("mySharedTeams/{accessShareId:Guid}/events")]
        public Task<List<TeamMemberEventOverviewModel>> GetSharedTeamMemberEventOverview([FromQuery] GetSharedTeamMemberEventOverviewRequest request)
        {
            return _teamCalendarSharingService.GetSharedTeamMemberEventOverview(request, CurrentUserId);
        }

        [HttpGet("mySharedTeams/{accessShareId:Guid}/members")]
        public Task<List<TeamMemberModel>> GetSharedTeamMembers([FromRoute] Guid accessShareId)
        {
            return _teamCalendarSharingService.GetSharedTeamMembers(accessShareId, CurrentUserId);
        }

        [HttpGet("mySharedTeams/{accessShareId:Guid}/members/{memberId:Guid}/events")]
        public Task<List<TeamMemberEventModel>> GetSharedTeamMemberEvents([FromQuery] GetSharedTeamMemberEventsRequest request)
        {
            return _teamCalendarSharingService.GetSharedTeamMemberEvents(request, CurrentUserId);
        }

        [HttpPost]
        public Task SaveCalendarAccessSharings([FromBody] SaveTeamCalendarAccessSharingsRequest request)
        {
            return _teamCalendarSharingService.SaveCalendarAccessSharings(request, CurrentUserId);
        }

        [HttpGet]
        public Task<PagedResultDto<UserAccessSharingModel>> GetCalendarAccessSharings([FromQuery] PagedResultRequestDto request)
        {
            return _teamCalendarSharingService.GetCalendarAccessSharings(CurrentUserId, request);
        }
    }
}
