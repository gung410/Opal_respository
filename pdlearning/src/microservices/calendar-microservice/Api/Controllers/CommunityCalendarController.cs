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
    [Route("api/calendars/communities")]
    public class CommunityCalendarController : ApplicationApiController
    {
        private readonly ICommunityCalendarApplicationService _communityCalendarApplicationService;

        public CommunityCalendarController(
            ICommunityCalendarApplicationService communityCalendarApplicationService,
            IUserContext userContext) : base(userContext)
        {
            _communityCalendarApplicationService = communityCalendarApplicationService;
        }

        [HttpGet("{communityId:guid}/events")]
        public Task<List<CommunityEventModel>> GetCommunityEvents([FromQuery] GetCommunityEventRequest dto, [FromRoute] Guid communityId)
        {
            return _communityCalendarApplicationService.GetCommunityEvents(dto, communityId);
        }

        [HttpGet("myEvents")]
        public Task<List<CommunityEventModel>> GetCommunityEventsOfCurrentUser([FromQuery] GetMyCommunityEventRequest request)
        {
            return _communityCalendarApplicationService.GetCommunityEventsByUser(request, CurrentUserId);
        }

        [HttpGet("events/{eventId:guid}")]
        public Task<CommunityEventModel> GetCommunityEventDetails(Guid eventId)
        {
            return _communityCalendarApplicationService.GetCommunityEvent(eventId);
        }

        [HttpPost("events")]
        public Task<CommunityEventModel> CreateCommunityEvent([FromBody] CreateCommunityEventRequest dto)
        {
            return _communityCalendarApplicationService.CreateCommunityEvent(dto, CurrentUserId);
        }

        [HttpPut("events")]
        public Task<CommunityEventModel> UpdateCommunityEvent([FromBody] UpdateCommunityEventRequest dto)
        {
            return _communityCalendarApplicationService.UpdateCommunityEvent(dto, CurrentUserId);
        }

        [HttpDelete("events/{eventId:guid}")]
        public Task DeleteCommunityEvent(Guid eventId)
        {
            return _communityCalendarApplicationService.DeleteCommunityEvent(eventId, CurrentUserId);
        }

        [HttpGet("{communityId:guid}/events/{eventType:CalendarEventSource}")]
        public Task<PagedResultDto<CommunityEventModel>> GetCommunityEventsByCommunityId(GetCommunityEventsByCommunityIdRequest request)
        {
            return _communityCalendarApplicationService.GetCommunityEventsByCommunityId(CurrentUserId, request);
        }

        [HttpPost("events/webinar")]
        public Task<CommunityEventModel> CreateWebinarCommunityEvent([FromBody] CreateCommunityEventRequest dto)
        {
            return _communityCalendarApplicationService.CreateWebinarCommunityEvent(dto, CurrentUserId);
        }

        [HttpPut("events/webinar")]
        public Task<CommunityEventModel> UpdateWebinarCommunityEvent([FromBody] UpdateCommunityEventRequest dto)
        {
            return _communityCalendarApplicationService.UpdateWebinarCommunityEvent(dto, CurrentUserId);
        }

        [HttpDelete("events/{eventId:guid}/webinar")]
        public Task DeleteWebinarCommunityEvent(Guid eventId)
        {
            return _communityCalendarApplicationService.DeleteWebinarCommunityEvent(eventId, CurrentUserId);
        }
    }
}
