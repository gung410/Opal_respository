using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microservice.LnaForm.Application.Commands;
using Microservice.LnaForm.Application.Models;
using Microservice.LnaForm.Application.Queries;
using Microservice.LnaForm.Application.RequestDtos;
using Microservice.LnaForm.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Thunder.Platform.AspNetCore.Controllers;
using Thunder.Platform.Core.Application.Dtos;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Cqrs;

namespace Microservice.LnaForm.Controllers
{
    [Route("api/forms")]
    public class FormController : ApplicationApiController
    {
        private readonly IThunderCqrs _thunderCqrs;
        private readonly IFormApplicationService _formApplicationService;

        public FormController(IThunderCqrs thunderCqrs, IUserContext userContext, IFormApplicationService formApplicationService) : base(userContext)
        {
            _thunderCqrs = thunderCqrs;
            _formApplicationService = formApplicationService;
        }

        [HttpPost("create")]
        public async Task<FormWithQuestionsModel> CreateForm([FromBody] CreateFormRequestDto dto)
        {
            return await _formApplicationService.CreateForm(dto, CurrentUserId);
        }

        [HttpPost("clone")]
        public async Task<FormWithQuestionsModel> CloneForm([FromBody] CloneFormRequestDto dto)
        {
            return await _formApplicationService.CloneForm(dto, CurrentUserId);
        }

        [HttpPost("update")]
        public async Task<FormWithQuestionsModel> UpdateForm([FromBody] UpdateFormRequestDto dto)
        {
            return await _formApplicationService.UpdateForm(dto, CurrentUserId);
        }

        [HttpPost("import")]
        public async Task ImportForm([FromBody] ImportFormRequest dto)
        {
            await _formApplicationService.ImportForms(dto, CurrentUserId);
        }

        [HttpPut("update-status-and-data")]
        public async Task<FormWithQuestionsModel> UpdateFormStatusAndData([FromBody] UpdateFormRequestDto dto)
        {
            return await _formApplicationService.UpdateFormStatusAndData(dto, CurrentUserId);
        }

        [HttpDelete("{formId:guid}")]
        public async Task DeleteForm(Guid formId)
        {
            await _formApplicationService.DeleteForm(formId, CurrentUserId);
        }

        [HttpPost("search")]
        public async Task<PagedResultDto<FormModel>> SearchForms([FromBody] SearchFormsRequestDto dto)
        {
            return await _formApplicationService.SearchForms(dto, CurrentUserId);
        }

        [HttpPost("search-questions")]
        public async Task<PagedResultDto<FormQuestionModel>> SearchFormQuestions([FromBody] SearchFormQuestionsRequestDto dto)
        {
            return await _formApplicationService.SearchFormQuestions(dto, CurrentUserId);
        }

        [HttpGet("{formId:guid}/full-with-questions-by-id")]
        public async Task<FormWithQuestionsModel> GetFormWithQuestionsById(Guid formId, [FromQuery] bool onlyPublished = false)
        {
            return await _formApplicationService.GetFormWithQuestionsById(
                new GetFormWithQuestionsByIdRequestDto { FormId = formId, OnlyPublished = onlyPublished },
                CurrentUserId);
        }

        [HttpGet("{formId:guid}/standalone")]
        public async Task<FormWithQuestionsModel> GetFormStandaloneById(Guid formId, [FromQuery] bool onlyPublished = false)
        {
            return await _formApplicationService.GetFormStandaloneById(
                new GetFormStandaloneByIdRequestDto { FormOriginalObjectId = formId, OnlyPublished = onlyPublished },
                CurrentUserId);
        }

        [HttpPut("transfer")]
        public async Task TransferOwnership([FromBody] TransferOwnershipRequest request)
        {
            await _formApplicationService.TransferCourseOwnership(request);
        }

        [HttpGet("version-tracking-form-data/{versionTrackingId:guid}")]
        public async Task<VersionTrackingFormDataModel> GetFormDataByVersionTrackingId(Guid versionTrackingId)
        {
            return await _formApplicationService.GetFormDataByVersionTrackingId(
                new GetFormDataByVersionTrackingIdRequestDto { VersionTrackingId = versionTrackingId },
                CurrentUserId);
        }

        [HttpPut("archive")]
        public async Task ArchiveForm([FromBody] ArchiveFormRequest request)
        {
            var command = new ArchiveFormCommand
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
