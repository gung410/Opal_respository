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
    [Route("api/session")]
    public class SessionController : BaseController<SessionService>
    {
        public SessionController(IUserContext userContext, SessionService appService) : base(userContext, appService)
        {
        }

        [HttpGet("{sessionId:guid}")]
        public async Task<SessionModel> GetSessionById(Guid sessionId)
        {
            return await AppService.GetSessionById(sessionId);
        }

        [HttpGet("byClassRunId")]
        public async Task<PagedResultDto<SessionModel>> GetSessionsByClassRunId(GetSessionsBySessionIdRequest request)
        {
            return await AppService.GetSessionsByClassRunId(request);
        }

        [HttpPost("byClassRunIds")]
        public async Task<IEnumerable<SessionModel>> GetSessionsByClassRunIds([FromBody] IEnumerable<Guid> classRunIds)
        {
            return await AppService.GetSessionsByClassRunIds(classRunIds);
        }

        [HttpPost("save")]
        public async Task<SessionModel> SaveSession([FromBody] SaveSessionRequest request)
        {
            return await AppService.SaveSession(request);
        }

        [HttpPost("changeLearningMethod")]
        public async Task<SessionModel> ChangeLearningMethod([FromBody] ChangeLearningMethodRequest request)
        {
            return await AppService.ChangeLearningMethod(request);
        }

        [HttpPost("getSessionsByIds")]
        public async Task<IEnumerable<SessionModel>> GetSessionsByIds([FromBody] IEnumerable<Guid> sessionIds)
        {
            return await AppService.GetSessionsByIds(sessionIds);
        }

        [HttpGet("{sessionId:guid}/code")]
        public async Task<SessionModel> GetSessionsCodeById(Guid sessionId)
        {
            return await AppService.GetSessionCodeById(sessionId);
        }

        [HttpPost("checkExistedSessionField")]
        public async Task<bool> CheckExistedSessionField([FromBody] CheckExistedSessionFieldRequest request)
        {
            return await AppService.CheckExistedSessionField(request);
        }

        [HttpDelete("{sessionId:guid}")]
        public async Task DeleteSession(Guid sessionId)
        {
            await AppService.DeleteSession(sessionId);
        }

        [HttpPost("getUpcomingSessionByClassRunIds")]
        public async Task<List<UpcomingSessionModel>> GetUpcomingSession([FromBody] IEnumerable<Guid> classRunIds)
        {
            return await AppService.GetUpcomingSessionByClassRunIds(classRunIds);
        }

        [HttpGet("maxMinutesCanJoinWebinarEarly")]
        public async Task<int> GetMaxMinutesCanJoinWebinarEarly()
        {
            return await AppService.GetMaxMinutesCanJoinWebinarEarly();
        }
    }
}
