using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microservice.Course.Application.Models;
using Microservice.Course.Application.RequestDtos;
using Microservice.Course.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Thunder.Platform.Core.Application.Dtos;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Exceptions;
using Thunder.Service.Authentication;

namespace Microservice.Course.Controllers
{
    [Route("api/classrun")]
    public class ClassRunController : BaseController<ClassRunService>
    {
        public ClassRunController(IUserContext userContext, ClassRunService appService) : base(userContext, appService)
        {
        }

        [HttpGet("{classRunId:guid}")]
        public async Task<ClassRunModel> GetClassRunById(Guid classRunId, [FromQuery] bool loadHasLearnerStarted = false)
        {
            return await AppService.GetClassRunById(classRunId, loadHasLearnerStarted);
        }

        [HttpPost("getByClassRunCodes")]
        public async Task<IEnumerable<ClassRunModel>> GetClassRunsByClassRunCodes([FromBody] GetClassRunsByClassRunCodesRequest request)
        {
            return await AppService.GetClassRunsByClassRunCodes(request);
        }

        [HttpPost("byCourseId")]
        public async Task<PagedResultDto<ClassRunModel>> GetClassRunsByCourseId([FromBody] GetClassRunsByCourseIdRequest request)
        {
            return await AppService.GetClassRunsByCourseId(request);
        }

        [HttpPost("save")]
        public async Task<ClassRunModel> SaveClassRun([FromBody] SaveClassRunRequest request)
        {
            return await AppService.SaveClassRun(request);
        }

        [HttpPut("changeStatus")]
        public async Task ChangeClassRunStatus([FromBody] ChangeClassRunStatusRequest request)
        {
            await AppService.ChangeStatus(request);
        }

        [HttpPut("changeCancellationStatus")]
        public async Task ChangeCancellationStatus([FromBody] ChangeClassRunCancellationStatusRequest request)
        {
            await AppService.ChangeCancellationStatus(request);
        }

        [HttpPut("changeRescheduleStatus")]
        public async Task ChangeRescheduleStatus([FromBody] ChangeClassRunRescheduleStatusRequest request)
        {
            await AppService.ChangeRescheduleStatus(request);
        }

        [HttpGet("checkClassIsFull")]
        public async Task<bool> CheckClassIsFull(Guid classRunId)
        {
            return await AppService.CheckClassIsFull(classRunId);
        }

        [HttpPost("getClassRunsByIds")]
        public async Task<IEnumerable<ClassRunModel>> GetClassRunsByIds([FromBody] IEnumerable<Guid> classRunIds)
        {
            return await AppService.GetClassRunsByIds(classRunIds);
        }

        [HttpPut("toggleCourseCriteria")]
        public async Task ToggleCourseCriteria([FromBody] ToggleCourseCriteriaRequest request)
        {
            await AppService.ToggleCourseCriteria(request);
        }

        [HttpPut("toggleCourseAutomate")]
        public async Task ToggleCourseAutomate([FromBody] ToggleCourseAutomateRequest request)
        {
            await AppService.ToggleCourseAutomate(request);
        }

        [HttpPost("migrateClassRunNotification")]
        public async Task<PagedResultDto<Guid>> MigrateClassRunNotification([FromBody] MigrateClassrunNotificationRequest request)
        {
            EnsureValidPermission(UserContext.IsSysAdministrator());

            return await AppService.MigrateClassRunNotification(request);
        }

        [HttpPost("getTotalParticipantInClassRun")]
        public async Task<IEnumerable<TotalParticipantInClassRunModel>> GetTotalParticipantInClassRun([FromBody] GetTotalParticipantInClassRunRequest request)
        {
            return await AppService.GetTotalParticipantInClassRun(request);
        }
    }
}
