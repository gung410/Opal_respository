using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Conexus.Opal.AccessControl.Domain.Constants.PermissionKeys;
using Microservice.Form.Application.Commands;
using Microservice.Form.Application.Models;
using Microservice.Form.Application.RequestDtos;
using Microservice.Form.Application.Services;
using Microservice.Form.Infrastructure.Attributes;
using Microsoft.AspNetCore.Mvc;
using Thunder.Platform.AspNetCore.Controllers;
using Thunder.Platform.Core.Application.Dtos;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Exceptions;
using Thunder.Platform.Cqrs;
using Thunder.Service.Authentication;

namespace Microservice.Form.Controllers
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

        [PermissionRequiredAttribute(CourseContentPermissionKeys.FormCreateForm)]
        [HttpPost("create")]
        public async Task<FormWithQuestionsModel> CreateForm([FromBody] CreateFormRequestDto dto)
        {
            return await _formApplicationService.CreateForm(dto, CurrentUserId);
        }

        [PermissionRequiredAttribute(CourseContentPermissionKeys.FormDuplicate)]
        [HttpPost("clone")]
        public async Task<FormWithQuestionsModel> CloneForm([FromBody] CloneFormRequestDto dto)
        {
            return await _formApplicationService.CloneForm(dto, CurrentUserId);
        }

        [PermissionRequiredAttribute(CourseContentPermissionKeys.FormDuplicate)]
        [HttpPost("clone-form-assessment")]
        public async Task<FormWithQuestionsModel> CloneFormAssessment([FromBody] CloneFormRequestDto dto)
        {
            return await _formApplicationService.CloneAssessmentForm(dto, CurrentUserId);
        }

        [PermissionRequiredAttribute(CourseContentPermissionKeys.FormEdit)]
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

        [PermissionRequiredUpdateStatusAttribute]
        [HttpPut("update-status-and-data")]
        public async Task<FormWithQuestionsModel> UpdateFormStatusAndData([FromBody] UpdateFormRequestDto dto)
        {
            return await _formApplicationService.UpdateFormStatusAndData(dto, CurrentUserId);
        }

        [PermissionRequiredAttribute(CourseContentPermissionKeys.FormDelete)]
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

        [HttpPost("getPendingApprovalForms")]
        public async Task<PagedResultDto<FormModel>> GetPendingApprovalForms([FromBody] GetPendingApprovalFormsRequestDto dto)
        {
            return await _formApplicationService.GetPendingApprovalForms(dto, CurrentUserId);
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

        [HttpGet("{formId:guid}/assessment-form")]
        public async Task<FormAssessmentModel> GetFormAssessment(Guid formId, [FromQuery] bool onlyPublished = false)
        {
            return await _formApplicationService.GetFormtAssessmentById(
                new GetFormAssessmentByIdRequestDto { FormId = formId, OnlyPublished = onlyPublished },
                CurrentUserId);
        }

        [HttpGet("{formId:guid}/standalone")]
        public async Task<FormWithQuestionsModel> GetFormStandaloneById(Guid formId, [FromQuery] bool onlyPublished = false)
        {
            return await _formApplicationService.GetFormStandaloneById(
                new GetFormStandaloneByIdRequestDto { FormOriginalObjectId = formId, OnlyPublished = onlyPublished },
                CurrentUserId);
        }

        [HttpGet("version-tracking-form-data/{versionTrackingId:guid}")]
        public async Task<VersionTrackingFormDataModel> GetFormDataByVersionTrackingId(Guid versionTrackingId)
        {
            return await _formApplicationService.GetFormDataByVersionTrackingId(
                new GetFormDataByVersionTrackingIdRequestDto { VersionTrackingId = versionTrackingId },
                CurrentUserId);
        }

        [PermissionRequiredAttribute(CourseContentPermissionKeys.FormTransferOwnerShip)]
        [HttpPut("transfer")]
        public async Task TransferOwnership([FromBody] TransferOwnershipRequest request)
        {
            await _formApplicationService.TransferCourseOwnership(request);
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

        [HttpPost("migrateSearchFormData")]
        public async Task<PagedResultDto<Guid>> MigrateSearchFormData([FromQuery] MigrateSearchEngineDataRequestDto reuest)
        {
            if (!UserContext.IsSysAdministrator())
            {
                throw new BusinessLogicException("You do not have permission to access this endpoint");
            }

            return await _formApplicationService.MigrateSearchFormData(reuest);
        }
    }
}
