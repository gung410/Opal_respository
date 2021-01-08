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
    [Route("api/calendars/personal")]
    public class PersonalCalendarController : ApplicationApiController
    {
        private readonly IPersonalCalendarApplicationService _calendarApplicationService;

        public PersonalCalendarController(
            IPersonalCalendarApplicationService calendarApplicationService,
            IUserContext userContext) : base(userContext)
        {
            _calendarApplicationService = calendarApplicationService;
        }

        [HttpGet("myEvents/range")]
        public Task<List<EventModel>> GetUserEventsByRange([FromQuery] GetPersonalEventByRangeRequest dto)
        {
            return _calendarApplicationService.GetPersonalCalendarEventsByRange(dto, CurrentUserId);
        }

        [HttpGet("myEvents/range/count")]
        public Task<int> CountUserEventsByRange([FromQuery] GetPersonalEventByRangeRequest dto)
        {
            return _calendarApplicationService.CountPersonalCalendarEventsByRange(dto, CurrentUserId);
        }

        [HttpGet("myEvents")]
        public Task<List<EventModel>> GetUserEvents([FromQuery] GetPersonalEventRequest dto)
        {
            return _calendarApplicationService.GetPersonalCalendarEvents(dto, CurrentUserId);
        }

        [HttpGet("events/{eventId:guid}")]
        public Task<PersonalEventDetailsModel> GetEventDetails(Guid eventId)
        {
            return _calendarApplicationService.GetEventDetails(eventId, CurrentUserId);
        }

        [HttpPost("events")]
        public Task<PersonalEventModel> CreateUserEvent([FromBody] CreatePersonalEventRequest dto)
        {
            return _calendarApplicationService.CreateEvent(dto, CurrentUserId);
        }

        [HttpPut("events")]
        public Task<PersonalEventModel> UpdateEvent([FromBody] UpdatePersonalEventRequest dto)
        {
            return _calendarApplicationService.UpdateEvent(dto, CurrentUserId);
        }

        [HttpDelete("events/{eventId:guid}")]
        public Task DeleteEvent(Guid eventId)
        {
            return _calendarApplicationService.DeleteEvent(eventId, CurrentUserId);
        }
    }
}
