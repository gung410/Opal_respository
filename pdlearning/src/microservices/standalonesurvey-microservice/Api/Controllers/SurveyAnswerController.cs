using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microservice.StandaloneSurvey.Application.Models;
using Microservice.StandaloneSurvey.Application.RequestDtos;
using Microservice.StandaloneSurvey.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Thunder.Platform.Core.Context;

namespace Microservice.StandaloneSurvey.Controllers
{
    [GettingSubModuleActionFilter]
    [Route("api/form-answers")]
    public class SurveyAnswerController : BaseController<SurveyAnswerApplicationService>
    {
        public SurveyAnswerController(IUserContext userContext, SurveyAnswerApplicationService appService) : base(userContext, appService)
        {
        }

        [HttpPost]
        public async Task<SurveyAnswerModel> SaveFormAnswer([FromBody] SaveSurveyAnswerRequestDto dto)
        {
            return await AppService.SaveFormAnswer(dto, CurrentUserId);
        }

        [HttpPost("update")]
        public async Task<SurveyAnswerModel> UpdateFormAnswer([FromBody] UpdateSurveyAnswerRequestDto dto)
        {
            return await AppService.UpdateFormAnswer(dto, CurrentUserId);
        }

        [HttpGet("form-ids/{formId:guid}")]
        public async Task<IEnumerable<SurveyAnswerModel>> GetByFormId(
            Guid formId,
            [FromQuery] Guid? resourceId,
            [FromQuery] Guid? userId)
        {
            return await AppService.GetBySurveyId(formId, resourceId, userId ?? CurrentUserId);
        }
    }
}
