using System;
using System.Threading.Tasks;
using Microservice.StandaloneSurvey.Application.Commands;
using Microservice.StandaloneSurvey.Application.Models;
using Microservice.StandaloneSurvey.Application.Queries;
using Microservice.StandaloneSurvey.Application.RequestDtos;
using Microservice.StandaloneSurvey.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Thunder.Platform.AspNetCore.Controllers;
using Thunder.Platform.Core.Application.Dtos;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Cqrs;

namespace Microservice.StandaloneSurvey.Controllers
{
    [GettingSubModuleActionFilter]
    [Route("api/forms")]
    public class StandaloneSurveyController : ApplicationApiController
    {
        private readonly IThunderCqrs _thunderCqrs;
        private readonly IStandaloneSurveyApplicationService _standaloneSurveyApplicationService;

        public StandaloneSurveyController(IThunderCqrs thunderCqrs, IUserContext userContext, IStandaloneSurveyApplicationService standaloneSurveyApplicationService) : base(userContext)
        {
            _thunderCqrs = thunderCqrs;
            _standaloneSurveyApplicationService = standaloneSurveyApplicationService;
        }

        [HttpPost("create")]
        public async Task<SurveyWithQuestionsModel> CreateSurveys([FromBody] CreateSurveyRequestDto dto)
        {
            return await _standaloneSurveyApplicationService.CreateSurveys(dto, CurrentUserId);
        }

        [HttpPost("clone")]
        public async Task<SurveyWithQuestionsModel> CloneSurveys([FromBody] CloneSurveyRequestDto dto)
        {
            return await _standaloneSurveyApplicationService.CloneSurveys(dto, CurrentUserId);
        }

        [HttpPost("update")]
        public async Task<SurveyWithQuestionsModel> UpdateSurveys([FromBody] UpdateSurveyRequestDto dto)
        {
            return await _standaloneSurveyApplicationService.UpdateSurveys(dto, CurrentUserId);
        }

        [HttpPost("import")]
        public async Task ImportStandaloneSurveys([FromBody] ImportStandaloneSurveyRequest dto)
        {
            await _standaloneSurveyApplicationService.ImportStandaloneSurveys(dto, CurrentUserId);
        }

        [HttpPut("update-status-and-data")]
        public async Task<SurveyWithQuestionsModel> UpdateSurveyStatusAndData([FromBody] UpdateSurveyRequestDto dto)
        {
            return await _standaloneSurveyApplicationService.UpdateSurveyStatusAndData(dto, CurrentUserId);
        }

        [HttpDelete("{formId:guid}")]
        public async Task DeleteSurvey(Guid formId)
        {
            await _standaloneSurveyApplicationService.DeleteSurvey(formId, CurrentUserId);
        }

        [HttpPost("search")]
        public async Task<PagedResultDto<StandaloneSurveyModel>> SearchSurveys([FromBody] SearchSurveysRequestDto dto)
        {
            return await _standaloneSurveyApplicationService.SearchSurveys(dto, CurrentUserId);
        }

        [HttpPost("search-questions")]
        public async Task<PagedResultDto<SurveyQuestionModel>> SearchSurveyQuestions([FromBody] SearchSurveyQuestionsRequestDto dto)
        {
            return await _standaloneSurveyApplicationService.SearchSurveyQuestions(dto, CurrentUserId);
        }

        [HttpGet("{formId:guid}/full-with-questions-by-id")]
        public async Task<SurveyWithQuestionsModel> GetSurveyWithQuestionsById(Guid formId, [FromQuery] bool onlyPublished = false)
        {
            return await _standaloneSurveyApplicationService.GetSurveyWithQuestionsById(
                new GetSurveyWithQuestionsByIdRequestDto { FormId = formId, OnlyPublished = onlyPublished },
                CurrentUserId);
        }

        [HttpGet("{formId:guid}/standalone")]
        public async Task<SurveyWithQuestionsModel> GetSurveyParticipantById(Guid formId, [FromQuery] bool onlyPublished = false)
        {
            return await _standaloneSurveyApplicationService.GetSurveyParticipantById(
                new GetSurveyParticipantByIdRequestDto { FormOriginalObjectId = formId, OnlyPublished = onlyPublished },
                CurrentUserId);
        }

        [HttpPut("transfer")]
        public async Task TransferOwnership([FromBody] TransferOwnershipRequest request)
        {
            await _standaloneSurveyApplicationService.TransferCourseOwnership(request);
        }

        [HttpGet("version-tracking-form-data/{versionTrackingId:guid}")]
        public async Task<VersionTrackingSurveyDataModel> GetSurveyDataByVersionTrackingId(Guid versionTrackingId)
        {
            return await _standaloneSurveyApplicationService.GetSurveyDataByVersionTrackingId(
                new GetSurveyDataByVersionTrackingIdRequestDto { VersionTrackingId = versionTrackingId },
                CurrentUserId);
        }

        [HttpPut("archive")]
        public async Task ArchiveSurvey([FromBody] ArchiveSurveyRequest request)
        {
            var command = new ArchiveSurveyCommand
            {
                FormId = request.ObjectId,
                ArchiveBy = request.ArchiveByUserId
            };

            await _thunderCqrs.SendCommand(command);
        }

        [HttpGet("newest-assigned-survey-link")]
        public Task<AssignedLinkModel> GetNewestAssignedSurveyLink()
        {
            return _thunderCqrs.SendQuery(new GetNewestAssignedSurveyLinkQuery { User = CurrentUserId });
        }
    }
}
