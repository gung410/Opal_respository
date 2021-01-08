using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microservice.LnaForm.Application.Models;
using Microservice.LnaForm.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Thunder.Platform.Core.Context;

namespace Microservice.LnaForm.Controllers
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
