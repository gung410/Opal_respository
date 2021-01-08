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
    [Route("api/form-question-answers")]
    public class SurveyQuestionAnswerController : BaseController<SurveyQuestionAnswerApplicationService>
    {
        public SurveyQuestionAnswerController(IUserContext userContext, SurveyQuestionAnswerApplicationService appService) : base(userContext, appService)
        {
        }

        [HttpGet("question-ids/{questionId:guid}/statistics")]
        public async Task<IEnumerable<SurveyQuestionAnswerStatisticsModel>> GetStatisticsByQuestionId(
            Guid questionId)
        {
            return await AppService.GetStatisticsByQuestionId(questionId);
        }
    }
}
