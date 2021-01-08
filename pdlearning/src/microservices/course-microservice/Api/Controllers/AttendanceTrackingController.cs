using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microservice.Course.Application.Models;
using Microservice.Course.Application.RequestDtos;
using Microservice.Course.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Thunder.Platform.Core.Application.Dtos;
using Thunder.Platform.Core.Context;

namespace Microservice.Course.Controllers
{
    [Route("api/attendancetracking")]
    public class AttendanceTrackingController : BaseController<AttendanceTrackingService>
    {
        public AttendanceTrackingController(IUserContext userContext, AttendanceTrackingService appService) : base(userContext, appService)
        {
        }

        [HttpPost("session")]
        public async Task<PagedResultDto<AttendanceTrackingModel>> GetAttendanceTrackingBySessionId([FromBody] GetAttendanceTrackingBySessionIdRequest request)
        {
            return await AppService.GetAttendanceTrackingBySessionId(request);
        }

        [HttpGet("{attendanceTrackingId:guid}")]
        public async Task<AttendanceTrackingModel> GetAttendanceTrackingById(Guid attendanceTrackingId)
        {
            return await AppService.GetAttendanceTrackingById(attendanceTrackingId);
        }

        [HttpGet("currentUser/{classRunId:guid}")]
        public async Task<List<AttendanceTrackingModel>> GetUserAttendanceTrackingByClassRunId(Guid classRunId)
        {
            return await AppService.GetUserAttendanceTrackingByClassRunId(classRunId);
        }

        [HttpPut("changeStatus")]
        public async Task ChangeAttendanceTrackingStatus([FromBody] ChangeAttendanceTrackingStatusRequest request)
        {
            await AppService.ChangeStatus(request);
        }

        [HttpPut("changeReason")]
        public async Task ChangeAttendanceTrackingReason([FromBody] ChangeAttendanceTrackingReasonForAbsenceRequest request)
        {
            await AppService.ChangeReasonForAbsence(request);
        }

        [HttpGet("ratioofpresents")]
        public async Task<List<AttendanceRatioOfPresentInfo>> GetAttendanceRatioOfPresents(GetAttendanceRatioOfPresentsRequest request)
        {
            return await AppService.GetAttendanceRatioOfPresents(request);
        }

        [HttpPut("learnerTakeAttendance")]
        public async Task<AttendanceTrackingModel> LearnerTakeAttendance([FromBody] LearnerTakeAttendanceRequest request)
        {
            return await AppService.UpdateTakeAttendaceCodeScanned(request);
        }
    }
}
