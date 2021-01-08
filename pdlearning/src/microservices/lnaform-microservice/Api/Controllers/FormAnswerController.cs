using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microservice.LnaForm.Application.Models;
using Microservice.LnaForm.Application.RequestDtos;
using Microservice.LnaForm.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Thunder.Platform.Core.Context;

namespace Microservice.LnaForm.Controllers
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

        [HttpPost("update")]
        public async Task<FormAnswerModel> UpdateFormAnswer([FromBody] UpdateFormAnswerRequestDto dto)
        {
            return await AppService.UpdateFormAnswer(dto, CurrentUserId);
        }

        [HttpGet("form-ids/{formId:guid}")]
        public async Task<IEnumerable<FormAnswerModel>> GetByFormId(
            Guid formId,
            [FromQuery] Guid? resourceId,
            [FromQuery] Guid? userId)
        {
            return await AppService.GetByFormId(formId, resourceId, userId ?? CurrentUserId);
        }
    }
}
