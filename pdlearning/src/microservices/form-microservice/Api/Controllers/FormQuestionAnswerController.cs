using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microservice.Form.Application.Models;
using Microservice.Form.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Thunder.Platform.Core.Context;

namespace Microservice.Form.Controllers
{
    [Route("api/form-question-answers")]
    public class FormQuestionAnswerController : BaseController<FormQuestionAnswerApplicationService>
    {
        public FormQuestionAnswerController(IUserContext userContext, FormQuestionAnswerApplicationService appService) : base(userContext, appService)
        {
        }

        [HttpGet("question-ids/{questionId:guid}/statistics")]
        public async Task<IEnumerable<FormQuestionAnswerStatisticsModel>> GetStatisticsByQuestionId(
            Guid questionId)
        {
            return await AppService.GetStatisticsByQuestionId(questionId);
        }
    }
}
