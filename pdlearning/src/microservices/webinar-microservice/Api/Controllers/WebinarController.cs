using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microservice.Webinar.Application.Models;
using Microservice.Webinar.Application.Services;
using Microservice.Webinar.Domain.Enums;
using Microsoft.AspNetCore.Mvc;
using Thunder.Platform.AspNetCore.Controllers;
using Thunder.Platform.Core.Context;

namespace Microservice.Webinar.Controllers
{
    [Route("api/webinar")]
    public class WebinarController : ApplicationApiController
    {
        private readonly IWebinarApplicationService _webinarApplicationService;

        public WebinarController(IWebinarApplicationService webinarApplicationService, IUserContext userContext) : base(userContext)
        {
            _webinarApplicationService = webinarApplicationService;
        }

        [HttpGet("sessions/{source}/{sessionId:guid}/joinUrl")]
        public Task<ResultGetJoinUrlModel> GetJoinUrl(BookingSource source, Guid sessionId)
        {
            return _webinarApplicationService.GetJoinUrl(sessionId, source, CurrentUserId);
        }

        [HttpPost("GetMeetingPreRecordings")]
        public Task<List<ResultGetMeetingPreRecordingModel>> GetMeetingPreRecordings([FromBody] List<Guid> meetingIds)
        {
            return _webinarApplicationService.GetMeetingPreRecordings(meetingIds);
        }
    }
}
