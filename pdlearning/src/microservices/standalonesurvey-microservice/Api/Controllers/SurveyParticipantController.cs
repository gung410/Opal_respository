using System.Threading.Tasks;
using Microservice.StandaloneSurvey.Application.Models;
using Microservice.StandaloneSurvey.Application.RequestDtos;
using Microservice.StandaloneSurvey.Application.Services;
using Microservice.StandaloneSurvey.Application.Services.SurveyParticipant.Dtos;
using Microsoft.AspNetCore.Mvc;
using Thunder.Platform.Core.Application.Dtos;
using Thunder.Platform.Core.Context;

namespace Microservice.StandaloneSurvey.Controllers
{
    [GettingSubModuleActionFilter]
    [Route("api/form-participant")]
    public class SurveyParticipantController : BaseController<SurveyParticipantApplicationService>
    {
        public SurveyParticipantController(IUserContext userContext, SurveyParticipantApplicationService appService) : base(userContext, appService)
        {
        }

        [HttpPost("assign-participants")]
        public async Task AssignSurveyParticipant([FromBody] AssignSurveyParticipantsDto dto)
        {
            await AppService.AssignSurveyParticipant(dto, CurrentUserId);
        }

        [HttpPost("update-participant-status")]
        public async Task UpdateSurveyParticipantStatus([FromBody] UpdateSurveyParticipantStatusDto dto)
        {
            await AppService.UpdateSurveyParticipantStatus(dto, CurrentUserId);
        }

        [HttpPost("form-id")]
        public async Task<PagedResultDto<SurveyParticipantModel>> GetSurveyParticipantsByFormId([FromBody] GetSurveyParticipantsByFormIdDto dto)
        {
            return await AppService.GetSurveyParticipantsByFormId(dto);
        }

        [HttpPut("delete")]
        public async Task DeleteSurveyParticipantsById([FromBody] DeleteSurveyParticipantsDto dto)
        {
            await AppService.DeleteSurveyParticipantsById(dto);
        }

        [HttpPost("remind")]
        public async Task RemindParticipants([FromBody] RemindSurveyParticipantRequest request)
        {
            await AppService.RemindSurveyParticipant(request);
        }
    }
}
