using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using Conexus.Opal.AccessControl.Infrastructure;
using Microservice.LnaForm.Application.Commands;
using Microservice.LnaForm.Application.Models;
using Microservice.LnaForm.Application.Queries;
using Microservice.LnaForm.Application.RequestDtos;
using Microservice.LnaForm.Common.Helpers;
using Microservice.LnaForm.Domain.ValueObjects.Form;
using Microservice.LnaForm.Domain.ValueObjects.Questions;
using Microservice.LnaForm.Versioning.Application.RequestDtos;
using Microservice.LnaForm.Versioning.Application.Services;
using Microservice.LnaForm.Versioning.Entities;
using Thunder.Platform.Core.Application;
using Thunder.Platform.Core.Application.Dtos;
using Thunder.Platform.Core.Domain.Entities;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;
using Thunder.Platform.Core.Timing;
using Thunder.Platform.Cqrs;
using FormEntity = Microservice.LnaForm.Domain.Entities.Form;

namespace Microservice.LnaForm.Application.Services
{
    public class FormApplicationService : ApplicationService, IFormApplicationService
    {
        private readonly IThunderCqrs _thunderCqrs;
        private readonly VersionTrackingApplicationService _versionTrackingApplicationService;
        private readonly IAccessControlContext _accessControlContext;
        private readonly IRepository<FormEntity> _formRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        public FormApplicationService(
            IThunderCqrs thunderCqrs,
            IUnitOfWorkManager unitOfWorkManager,
            IAccessControlContext accessControlContext,
            IRepository<FormEntity> formRepository,
            VersionTrackingApplicationService versionTrackingApplicationService)
        {
            _thunderCqrs = thunderCqrs;
            _unitOfWorkManager = unitOfWorkManager;
            _versionTrackingApplicationService = versionTrackingApplicationService;
            _accessControlContext = accessControlContext;
            _formRepository = formRepository;
        }

        public async Task<FormWithQuestionsModel> CreateForm(CreateFormRequestDto dto, Guid userId)
        {
            var formId = Guid.NewGuid();
            var command = new SaveFormCommand
            {
                IsCreation = true,
                Form = dto.ToFormModel(formId),
                SaveFormQuestionCommands = dto.FormQuestions.Select(p => p.BuildSaveFormQuestionCommand(formId)),
                FormSections = dto.FormSections.Select(section => section.BuildFormSectionCommand()),
                UserId = userId,
                DepartmentId = _accessControlContext.GetUserDepartment(),
                IsAutoSave = dto.IsAutoSave
            };
            await _thunderCqrs.SendCommand(command);

            await _versionTrackingApplicationService.CreateVersionTracking(new CreateVersionTrackingParameter()
            {
                VersionSchemaType = VersionSchemaType.LnaForm,
                ObjectId = formId,
                UserId = command.UserId,
                ActionComment = $"Created \"{dto.Title}\"",
                RevertObjectId = Guid.Empty,
                CanRollback = false,
                IncreaseMajorVersion = false,
                IncreaseMinorVersion = true,
            });
            return await _thunderCqrs.SendQuery(new GetFormWithQuestionsByIdQuery { FormId = formId, UserId = userId });
        }

        public async Task ImportForms(ImportFormRequest dtos, Guid userId)
        {
            var command = new ImportFormCommand
            {
                UserId = userId,
                DepartmentId = _accessControlContext.GetUserDepartment(),
                FormWithSectionsQuestions = dtos.FormWithQuestionsSections
            };

            await _thunderCqrs.SendCommand(command);
        }

        public async Task<FormWithQuestionsModel> CloneForm(CloneFormRequestDto dto, Guid userId)
        {
            var originalForm = await _formRepository.FirstOrDefaultAsync(p => p.Id == dto.FormId);
            if (originalForm == null)
            {
                throw new FormAccessDeniedException();
            }

            var newId = Guid.NewGuid();
            var command = new CloneFormAsNewVersionCommand
            {
                Id = originalForm.Id,
                UserId = userId,
                NewId = newId,
                ParentId = Guid.Empty,
                Status = FormStatus.Draft,
                NewTitle = dto.NewTitle,
                IsCloneToNewVersion = false
            };

            await _thunderCqrs.SendCommand(command);

            var newForm = await _thunderCqrs.SendQuery(new GetFormWithQuestionsByIdQuery { FormId = newId, UserId = userId });

            await _versionTrackingApplicationService.CreateVersionTracking(new CreateVersionTrackingParameter()
            {
                VersionSchemaType = VersionSchemaType.LnaForm,
                ObjectId = newForm.Form.Id,
                UserId = userId,
                ActionComment = $"Cloned from \"{originalForm.Title}\"",
                RevertObjectId = Guid.Empty,
                CanRollback = false,
                IncreaseMajorVersion = false,
                IncreaseMinorVersion = false,
            });

            return newForm;
        }

        public async Task<FormWithQuestionsModel> UpdateForm(UpdateFormRequestDto dto, Guid userId)
        {
            var command = new SaveFormCommand
            {
                IsCreation = false,
                Form = dto.ToFormModel(),
                SaveFormQuestionCommands = dto.ToSaveFormQuestions.Select(p => p.BuildSaveFormCommand(dto.Id)),
                ToDeleteFormQuestionIds = dto.ToDeleteFormQuestionIds,
                FormSections = dto.FormSections.Select(section => section.BuildFormSectionCommand()),
                ToDeleteFormSectionIds = dto.ToDeleteFormSectionIds,
                UserId = userId,
                IsAutoSave = dto.IsAutoSave
            };
            await _thunderCqrs.SendCommand(command);

            return await _thunderCqrs.SendQuery(new GetFormWithQuestionsByIdQuery { FormId = dto.Id, UserId = userId });
        }

        public async Task<FormWithQuestionsModel> UpdateFormStatusAndData(UpdateFormRequestDto dto, Guid userId)
        {
            var command = new UpdateFormAndChangeStatusCommand
            {
                NewFormID = Guid.NewGuid(),
                IsCreation = false,
                Form = dto.ToFormModel(),
                SaveFormQuestionCommands = dto.ToSaveFormQuestions.Select(p => p.BuildSaveFormCommand(dto.Id)),
                ToDeleteFormQuestionIds = dto.ToDeleteFormQuestionIds,
                FormSections = dto.FormSections.Select(section => section.BuildFormSectionCommand()),
                ToDeleteFormSectionIds = dto.ToDeleteFormSectionIds,
                UserId = userId,
                IsAutoSave = dto.IsAutoSave,
                IsUpdateToNewVersion = dto.IsUpdateToNewVersion
            };

            Guid formId = dto.Id;

            // To ignore the case update form to preview with status Unpublished
            if (dto.Status == FormStatus.Unpublished && !dto.IsAutoSave)
            {
                formId = command.NewFormID;
            }

            await _thunderCqrs.SendCommand(command);

            return await _thunderCqrs.SendQuery(new GetFormWithQuestionsByIdQuery { FormId = formId, UserId = userId });
        }

        public async Task DeleteForm(Guid formId, Guid userId)
        {
            var deleteFromCommand = new DeleteFormCommand
            {
                FormId = formId,
                UserId = userId
            };

            await _versionTrackingApplicationService.CreateVersionTracking(new CreateVersionTrackingParameter()
            {
                VersionSchemaType = VersionSchemaType.LnaForm,
                ObjectId = formId,
                UserId = userId,
                ActionComment = "Deleted",
                RevertObjectId = Guid.Empty,
                CanRollback = false,
                IncreaseMajorVersion = false,
                IncreaseMinorVersion = false,
            });

            await _thunderCqrs.SendCommand(deleteFromCommand);
        }

        public Task<PagedResultDto<FormModel>> SearchForms(SearchFormsRequestDto dto, Guid userId)
        {
            return _thunderCqrs.SendQuery(new SearchFormsQuery
            {
                FilterByStatus = dto.FilterByStatus,
                PagedInfo = dto.PagedInfo,
                SearchFormTitle = dto.SearchFormTitle,
                UserId = userId,
                IncludeFormForImportToCourse = dto.IncludeFormForImportToCourse
            });
        }

        public Task<PagedResultDto<FormQuestionModel>> SearchFormQuestions(SearchFormQuestionsRequestDto dto, Guid userId)
        {
            return _thunderCqrs.SendQuery(new SearchFormQuestionsQuery
            {
                FormId = dto.FormId,
                UserId = userId,
                PagedInfo = dto.PagedInfo
            });
        }

        public Task<FormWithQuestionsModel> GetFormWithQuestionsById(GetFormWithQuestionsByIdRequestDto dto, Guid userId)
        {
            return _thunderCqrs.SendQuery(new GetFormWithQuestionsByIdQuery
            {
                FormId = dto.FormId,
                UserId = userId,
                OnlyPublished = dto.OnlyPublished
            });
        }

        public Task<FormWithQuestionsModel> GetFormStandaloneById(GetFormStandaloneByIdRequestDto dto, Guid userId)
        {
            return _thunderCqrs.SendQuery(new GetFormStandaloneByIdQuery
            {
                FormOriginalObjectId = dto.FormOriginalObjectId,
                UserId = userId
            });
        }

        public Task TransferCourseOwnership(TransferOwnershipRequest request)
        {
            return this._thunderCqrs.SendCommand(new TransferOwnershipCommand { Request = request });
        }

        public Task<VersionTrackingFormDataModel> GetFormDataByVersionTrackingId(GetFormDataByVersionTrackingIdRequestDto dto, Guid userId)
        {
            return _thunderCqrs.SendQuery(new GetFormDataByVersionTrackingIdQuery
            {
                VersionTrackingId = dto.VersionTrackingId,
                UserId = userId
            });
        }
    }
}
