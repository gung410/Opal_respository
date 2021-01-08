using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microservice.Webinar.Application.Models;
using Microservice.Webinar.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Thunder.Platform.AspNetCore.Controllers;
using Thunder.Platform.Core.Context;

namespace Microservice.Webinar.Controllers
{
    [Route("api/records")]
    public class RecordController : ApplicationApiController
    {
        private readonly IRecordApplicationService _recordApplicationService;

        public RecordController(IRecordApplicationService recordApplicationService, IUserContext userContext) : base(userContext)
        {
            _recordApplicationService = recordApplicationService;
        }

        [HttpGet("bookings/{bookingSourceId:guid}")]
        public Task<List<RecordModel>> GetRecordsByMeeting(Guid bookingSourceId)
        {
            return _recordApplicationService.GetRecordsByBookingSourceId(bookingSourceId);
        }
    }
}
