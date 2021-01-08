using System;
using System.Linq;
using System.Threading.Tasks;
using Conexus.Opal.AccessControl.Infrastructure;
using Microservice.StandaloneSurvey.Application.Commands;
using Microservice.StandaloneSurvey.Application.Models;
using Microservice.StandaloneSurvey.Application.Queries;
using Microservice.StandaloneSurvey.Application.RequestDtos;
using Microservice.StandaloneSurvey.Domain.ValueObjects.Survey;
using Microservice.StandaloneSurvey.Versioning.Application.RequestDtos;
using Microservice.StandaloneSurvey.Versioning.Application.Services;
using Microservice.StandaloneSurvey.Versioning.Entities;
using Thunder.Platform.Core.Application;
using Thunder.Platform.Core.Application.Dtos;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;
using Thunder.Platform.Cqrs;

namespace Microservice.StandaloneSurvey.Application.Services
{
    public class StandaloneSurveyApplicationService : ApplicationService, IStandaloneSurveyApplicationService
    {
        private readonly IThunderCqrs _thunderCqrs;
        private readonly VersionTrackingApplicationService _versionTrackingApplicationService;
        private readonly IAccessControlContext _accessControlContext;
        private readonly IRepository<Domain.Entities.StandaloneSurvey> _formRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        public StandaloneSurveyApplicationService(
            IThunderCqrs thunderCqrs,
            IUnitOfWorkManager unitOfWorkManager,
            IAccessControlContext accessControlContext,
            IRepository<Domain.Entities.StandaloneSurvey> formRepository,
            VersionTrackingApplicationService versionTrackingApplicationService)
        {
            _thunderCqrs = thunderCqrs;
            _unitOfWorkManager = unitOfWorkManager;
            _versionTrackingApplicationService = versionTrackingApplicationService;
            _accessControlContext = accessControlContext;
            _formRepository = formRepository;
        }

        public async Task<SurveyWithQuestionsModel> CreateSurveys(CreateSurveyRequestDto dto, Guid userId)
        {
            var formId = Guid.NewGuid();
            var command = new SaveSurveyCommand
            {
                IsCreation = true,
                StandaloneSurvey = dto.ToStandaloneSurveyModel(formId),
                SaveFormQuestionCommands = dto.FormQuestions.Select(p => p.BuildSaveFormQuestionCommand(formId)),
                FormSections = dto.FormSections.Select(section => section.BuildSurveySectionCommand()),
                UserId = userId,
                DepartmentId = _accessControlContext.GetUserDepartment(),
                IsAutoSave = dto.IsAutoSave
            };
            await _thunderCqrs.SendCommand(command);

            await _versionTrackingApplicationService.CreateVersionTracking(new CreateVersionTrackingParameter()
            {
                VersionSchemaType = VersionSchemaType.StandaloneSurvey,
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

        public async Task ImportStandaloneSurveys(ImportStandaloneSurveyRequest dtos, Guid userId)
        {
            var command = new ImportFormCommand
            {
                UserId = userId,
                DepartmentId = _accessControlContext.GetUserDepartment(),
                FormWithSectionsQuestions = dtos.FormWithQuestionsSections
            };

            await _thunderCqrs.SendCommand(command);
        }

        public async Task<SurveyWithQuestionsModel> CloneSurveys(CloneSurveyRequestDto dto, Guid userId)
        {
            var originalForm = await _formRepository.FirstOrDefaultAsync(p => p.Id == dto.FormId);
            if (originalForm == null)
            {
                throw new SurveyAccessDeniedException();
            }

            var newId = Guid.NewGuid();
            var command = new CloneSurveyAsNewVersionCommand
            {
                Id = originalForm.Id,
                UserId = userId,
                NewId = newId,
                ParentId = Guid.Empty,
                Status = SurveyStatus.Draft,
                NewTitle = dto.NewTitle,
                IsCloneToNewVersion = false
            };

            await _thunderCqrs.SendCommand(command);

            var newForm = await _thunderCqrs.SendQuery(new GetFormWithQuestionsByIdQuery { FormId = newId, UserId = userId });

            await _versionTrackingApplicationService.CreateVersionTracking(new CreateVersionTrackingParameter()
            {
                VersionSchemaType = VersionSchemaType.StandaloneSurvey,
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

        public async Task<SurveyWithQuestionsModel> UpdateSurveys(UpdateSurveyRequestDto dto, Guid userId)
        {
            var command = new SaveSurveyCommand
            {
                IsCreation = false,
                StandaloneSurvey = dto.ToStandaloneSurveyModel(),
                SaveFormQuestionCommands = dto.ToSaveFormQuestions.Select(p => p.BuildSaveSurveyCommand(dto.Id)),
                ToDeleteFormQuestionIds = dto.ToDeleteFormQuestionIds,
                FormSections = dto.FormSections.Select(section => section.BuildSurveySectionCommand()),
                ToDeleteFormSectionIds = dto.ToDeleteFormSectionIds,
                UserId = userId,
                IsAutoSave = dto.IsAutoSave
            };
            await _thunderCqrs.SendCommand(command);

            return await _thunderCqrs.SendQuery(new GetFormWithQuestionsByIdQuery { FormId = dto.Id, UserId = userId });
        }

        public async Task<SurveyWithQuestionsModel> UpdateSurveyStatusAndData(UpdateSurveyRequestDto dto, Guid userId)
        {
            var command = new UpdateSurveyAndChangeStatusCommand
            {
                NewFormID = Guid.NewGuid(),
                IsCreation = false,
                StandaloneSurvey = dto.ToStandaloneSurveyModel(),
                SaveFormQuestionCommands = dto.ToSaveFormQuestions.Select(p => p.BuildSaveSurveyCommand(dto.Id)),
                ToDeleteFormQuestionIds = dto.ToDeleteFormQuestionIds,
                FormSections = dto.FormSections.Select(section => section.BuildSurveySectionCommand()),
                ToDeleteFormSectionIds = dto.ToDeleteFormSectionIds,
                UserId = userId,
                IsAutoSave = dto.IsAutoSave,
                IsUpdateToNewVersion = dto.IsUpdateToNewVersion
            };

            Guid formId = dto.Id;

            // To ignore the case update form to preview with status Unpublished
            if (dto.Status == SurveyStatus.Unpublished && !dto.IsAutoSave)
            {
                formId = command.NewFormID;
            }

            await _thunderCqrs.SendCommand(command);

            return await _thunderCqrs.SendQuery(new GetFormWithQuestionsByIdQuery { FormId = formId, UserId = userId });
        }

        public async Task DeleteSurvey(Guid formId, Guid userId)
        {
            var deleteFromCommand = new DeleteFormCommand
            {
                FormId = formId,
                UserId = userId
            };

            await _versionTrackingApplicationService.CreateVersionTracking(new CreateVersionTrackingParameter()
            {
                VersionSchemaType = VersionSchemaType.StandaloneSurvey,
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

        public Task<PagedResultDto<StandaloneSurveyModel>> SearchSurveys(SearchSurveysRequestDto dto, Guid userId)
        {
            return _thunderCqrs.SendQuery(new SearchFormsQuery
            {
                FilterByStatus = dto.FilterByStatus,
                PagedInfo = dto.PagedInfo,
                SearchFormTitle = dto.SearchFormTitle,
                UserId = userId,
                IncludeFormForImportToCourse = dto.IncludeFormForImportToCourse,
                IsOnlyCslSurveysForManagement = dto.OnlyCslSurveysForManagement
            });
        }

        public Task<PagedResultDto<SurveyQuestionModel>> SearchSurveyQuestions(SearchSurveyQuestionsRequestDto dto, Guid userId)
        {
            return _thunderCqrs.SendQuery(new SearchFormQuestionsQuery
            {
                FormId = dto.FormId,
                UserId = userId,
                PagedInfo = dto.PagedInfo
            });
        }

        public Task<SurveyWithQuestionsModel> GetSurveyWithQuestionsById(GetSurveyWithQuestionsByIdRequestDto dto, Guid userId)
        {
            return _thunderCqrs.SendQuery(new GetFormWithQuestionsByIdQuery
            {
                FormId = dto.FormId,
                UserId = userId,
                OnlyPublished = dto.OnlyPublished
            });
        }

        public Task<SurveyWithQuestionsModel> GetSurveyParticipantById(GetSurveyParticipantByIdRequestDto dto, Guid userId)
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

        public Task<VersionTrackingSurveyDataModel> GetSurveyDataByVersionTrackingId(GetSurveyDataByVersionTrackingIdRequestDto dto, Guid userId)
        {
            return _thunderCqrs.SendQuery(new GetFormDataByVersionTrackingIdQuery
            {
                VersionTrackingId = dto.VersionTrackingId,
                UserId = userId
            });
        }
    }
}
