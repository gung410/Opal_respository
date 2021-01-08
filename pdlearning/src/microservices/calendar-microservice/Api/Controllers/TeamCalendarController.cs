using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microservice.Calendar.Application.Models;
using Microservice.Calendar.Application.RequestDtos;
using Microservice.Calendar.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Thunder.Platform.AspNetCore.Controllers;
using Thunder.Platform.Core.Context;

namespace Microservice.Calendar.Controllers
{
    [Route("api/calendars/team")]
    public class TeamCalendarController : ApplicationApiController
    {
        private readonly ITeamCalendarApplicationService _teamCalendarService;

        public TeamCalendarController(
            ITeamCalendarApplicationService teamCalendarService,
            IUserContext userContext)
            : base(userContext)
        {
            _teamCalendarService = teamCalendarService;
        }

        [HttpGet("events")]
        public Task<List<TeamMemberEventOverviewModel>> GetTeamMemberEventOverview([FromQuery] GetTeamMemberEventOverviewRequest request)
        {
            return _teamCalendarService.GetTeamMemberEventOverview(CurrentUserId, request);
        }

        [HttpGet("members/{memberId:guid}/events")]
        public Task<List<TeamMemberEventModel>> GetTeamMemberEvents(GetTeamMemberEventsRequest request)
        {
            return _teamCalendarService.GetTeamMemberEvents(request, CurrentUserId);
        }

        [HttpGet("members")]
        public Task<List<TeamMemberModel>> GetTeamMembers()
        {
            return _teamCalendarService.GetTeamMembers(CurrentUserId);
        }
    }
}
