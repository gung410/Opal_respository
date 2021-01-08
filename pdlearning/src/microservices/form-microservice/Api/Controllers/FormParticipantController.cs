using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microservice.Form.Application.Models;
using Microservice.Form.Application.RequestDtos;
using Microservice.Form.Application.Services;
using Microservice.Form.Application.Services.FormParticipant.Dtos;
using Microsoft.AspNetCore.Mvc;
using Thunder.Platform.Core.Application.Dtos;
using Thunder.Platform.Core.Context;

namespace Microservice.Form.Controllers
{
    [Route("api/form-participant")]
    public class FormParticipantController : BaseController<FormParticipantApplicationService>
    {
        public FormParticipantController(IUserContext userContext, FormParticipantApplicationService appService) : base(userContext, appService)
        {
        }

        [HttpPost("assign-participants")]
        public async Task AssignFormParticipants([FromBody] AssignFormParticipantsDto dto)
        {
            await AppService.AssignFormParticipant(dto, CurrentUserId);
        }

        [HttpPost("update-participant-status")]
        public async Task UpdateFormParticipantStatus([FromBody] UpdateFormParticipantStatusDto dto)
        {
            await AppService.UpdateFormParticipantStatus(dto, CurrentUserId);
        }

        [HttpGet("my-participant-data/{formId}")]
        public async Task<FormParticipantModel> GetFormParticipantsByUserId(Guid formId)
        {
            return await AppService.GetMyParticipantData(formId);
        }

        [HttpPost("form-ids")]
        public async Task<IEnumerable<FormParticipantFormModel>> GetFormParticipantsByFormIds([FromBody] GetFormParticipantsByFormIdsDto dto)
        {
            return await AppService.GetFormParticipantsByFormIds(dto);
        }

        [HttpPost("form-id")]
        public async Task<PagedResultDto<FormParticipantModel>> GetFormParticipantsByFormId([FromBody] GetFormParticipantsByFormIdDto dto)
        {
            return await AppService.GetFormParticipantsByFormId(dto);
        }

        [HttpPut("delete")]
        public async Task DeleteFormParticipant([FromBody] DeleteFormParticipantsDto dto)
        {
            await AppService.DeleteFormParticipantsById(dto);
        }

        [HttpPost("remind")]
        public async Task RemindParticipants([FromBody] RemindFormParticipantRequest request)
        {
            await AppService.RemindFormParticipant(request);
        }
    }
}
