using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microservice.StandaloneSurvey.Application.Models;
using Microservice.StandaloneSurvey.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Thunder.Platform.Core.Context;

namespace Microservice.StandaloneSurvey.Controllers
{
    [GettingSubModuleActionFilter]
    [Route("api/form-sections")]
    public class SurveySectionController : BaseController<SurveySectionApplicationService>
    {
        public SurveySectionController(IUserContext userContext, SurveySectionApplicationService appService) : base(userContext, appService)
        {
        }

        [HttpPost]
        public async Task<SurveySectionModel> CreateSurveySection([FromBody] CreateSurveySectionRequestDto dto)
        {
            return await AppService.CreateSurveySection(dto, CurrentUserId);
        }

        [HttpGet("survey-id/{formId:guid}")]
        public async Task<List<SurveySectionModel>> GetFormSectionsByFormId([FromRoute]Guid formId)
        {
            return await AppService.GetFormSectionsBySurveyId(formId);
        }
    }
}
