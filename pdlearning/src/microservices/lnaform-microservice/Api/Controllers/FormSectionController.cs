using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microservice.LnaForm.Application.Models;
using Microservice.LnaForm.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Thunder.Platform.Core.Context;

namespace Microservice.LnaForm.Controllers
{
    [Route("api/form-sections")]
    public class FormSectionController : BaseController<FormSectionApplicationService>
    {
        public FormSectionController(IUserContext userContext, FormSectionApplicationService appService) : base(userContext, appService)
        {
        }

        [HttpPost]
        public async Task<FormSectionModel> CreateFormSection([FromBody] CreateFormSectionRequestDto dto)
        {
            return await AppService.CreateFormSection(dto, CurrentUserId);
        }

        [HttpGet("form-id/{formId:guid}")]
        public async Task<List<FormSectionModel>> GetFormSectionsByFormId([FromRoute]Guid formId)
        {
            return await AppService.GetFormSectionsByFormId(formId);
        }
    }
}
