using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microservice.Form.Application.Models;
using Microservice.Form.Application.RequestDtos;
using Microservice.Form.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Thunder.Platform.Core.Application.Dtos;
using Thunder.Platform.Core.Context;

namespace Microservice.Form.Controllers
{
    [Route("api/form-answers")]
    public class FormAnswerController : BaseController<FormAnswerApplicationService>
    {
        public FormAnswerController(IUserContext userContext, FormAnswerApplicationService appService) : base(userContext, appService)
        {
        }

        [HttpPost]
        public async Task<FormAnswerModel> SaveFormAnswer([FromBody] SaveFormAnswerRequestDto dto)
        {
            return await AppService.SaveFormAnswer(dto, CurrentUserId);
        }

        [HttpPost("search")]
        public async Task<PagedResultDto<FormAnswerModel>> SearchFormAnswer([FromBody] SearchFormAnswerRequestDto dto)
        {
            return await AppService.SearchFormAnswer(dto);
        }

        [HttpPost("update")]
        public async Task<FormAnswerModel> UpdateFormAnswer([FromBody] UpdateFormAnswerRequestDto dto)
        {
            return await AppService.UpdateFormAnswer(dto, CurrentUserId);
        }

        [HttpPost("update-score")]
        public async Task<FormAnswerModel> UpdateFormAnswerScoring([FromBody] UpdateFormAnswerScoreRequestDto dto)
        {
            return await AppService.UpdateFormAnswerScore(dto, CurrentUserId);
        }

        [HttpGet("form-ids/{formId:guid}")]
        public async Task<IEnumerable<FormAnswerModel>> GetByFormId(
            Guid formId,
            [FromQuery] Guid? resourceId,
            [FromQuery] Guid? myCourseId,
            [FromQuery] Guid? classRunId,
            [FromQuery] Guid? assignmentId,
            [FromQuery] Guid? userId)
        {
            return await AppService.GetByFormId(formId, resourceId, myCourseId, classRunId, assignmentId, userId ?? CurrentUserId);
        }
    }
}
