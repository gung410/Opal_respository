using System;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Conexus.Opal.AccessControl.Extensions;
using Conexus.Opal.AccessControl.Infrastructure;
using Microservice.Form.Application.Events;
using Microservice.Form.Application.Services;
using Microservice.Form.Common.Extensions;
using Microservice.Form.Domain.Entities;
using Microservice.Form.Domain.Services;
using Microservice.Form.Domain.ValueObjects.Form;
using Microservice.Form.Infrastructure;
using Microservice.Form.Versioning.Application.RequestDtos;
using Microservice.Form.Versioning.Application.Services;
using Microservice.Form.Versioning.Entities;
using Microservice.Form.Versioning.Extensions;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;
using Thunder.Platform.Core.Timing;
using Thunder.Platform.Cqrs;
using FormEntity = Microservice.Form.Domain.Entities.Form;

namespace Microservice.Form.Application.Commands
{
    public class SaveFormCommandHandler : BaseCommandHandler<SaveFormCommand>
    {
        private readonly IThunderCqrs _thunderCqrs;
        private readonly IRepository<FormEntity> _formRepository;
        private readonly IRepository<FormQuestion> _formQuestionRepository;
        private readonly IRepository<FormAnswer> _formAnswerRepository;
        private readonly IFormBusinessLogicService _formBusinessLogicService;
        private readonly VersionTrackingApplicationService _versionTrackingApplicationService;
        private readonly IFormUrlExtractor _formUrlExtractor;
        private readonly IRepository<AccessRight> _accessRightRepository;
        private readonly IRepository<FormSection> _formSectionRepository;
        private readonly IFormAnswerScoreCalculationService _formAnswerScoreCalculationService;

        public SaveFormCommandHandler(
            IThunderCqrs thunderCqrs,
            IRepository<FormEntity> formRepository,
            IRepository<FormQuestion> formQuestionRepository,
            IRepository<FormAnswer> formAnswerRepository,
            IFormBusinessLogicService formBusinessLogicService,
            IAccessControlContext accessControlContext,
            IRepository<AccessRight> accessRightRepository,
            IFormAnswerScoreCalculationService formAnswerScoreCalculationService,
            VersionTrackingApplicationService versionTrackingApplicationService,
            IFormUrlExtractor formUrlExtractor,
            IRepository<FormSection> formSectionRepository,
            IUnitOfWorkManager unitOfWorkManager) : base(unitOfWorkManager, accessControlContext)
        {
            _thunderCqrs = thunderCqrs;
            _formRepository = formRepository;
            _formQuestionRepository = formQuestionRepository;
            _formAnswerRepository = formAnswerRepository;
            _formBusinessLogicService = formBusinessLogicService;
            _versionTrackingApplicationService = versionTrackingApplicationService;
            _accessRightRepository = accessRightRepository;
            _formUrlExtractor = formUrlExtractor;
            _formSectionRepository = formSectionRepository;
            _formAnswerScoreCalculationService = formAnswerScoreCalculationService;
        }

        protected override async Task HandleAsync(SaveFormCommand command, CancellationToken cancellationToken)
        {
            if (command.IsCreation)
            {
                await CreateNewForm(command, cancellationToken);
            }
            else
            {
                await UpdateForm(command, cancellationToken);
            }
        }

        private async Task CreateNewForm(SaveFormCommand command, CancellationToken cancellationToken)
        {
            var form = new FormEntity
            {
                Id = command.Form.Id,
                OwnerId = command.UserId,
                Title = command.Form.Title,
                Status = command.Form.Status,
                Type = command.Form.Type,
                SurveyType = command.Form.SurveyType,
                RandomizedQuestions = command.Form.RandomizedQuestions,
                MaxAttempt = command.Form.MaxAttempt,
                PassingMarkPercentage = command.Form.PassingMarkPercentage,
                PassingMarkScore = command.Form.PassingMarkScore,
                InSecondTimeLimit = command.Form.InSecondTimeLimit,
                CreatedBy = command.UserId,
                CreatedDate = Clock.Now,
                PrimaryApprovingOfficerId = command.Form.PrimaryApprovingOfficerId,
                AlternativeApprovingOfficerId = command.Form.AlternativeApprovingOfficerId,
                IsAllowedDisplayPollResult = command.Form.IsAllowedDisplayPollResult,
                ParentId = command.Form.ParentId,
                OriginalObjectId = command.Form.OriginalObjectId == Guid.Empty ? command.Form.Id : command.Form.OriginalObjectId,
                DepartmentId = command.DepartmentId,
                IsShowFreeTextQuestionInPoll = command.Form.IsShowFreeTextQuestionInPoll,
                AnswerFeedbackDisplayOption = command.Form.AnswerFeedbackDisplayOption,
                AttemptToShowFeedback = command.Form.AttemptToShowFeedback,
                SqRatingType = command.Form.SqRatingType,
                StartDate = command.Form.StartDate,
                EndDate = command.Form.EndDate,
                IsStandalone = command.Form.IsStandalone,
                StandaloneMode = command.Form.StandaloneMode,
                FormRemindDueDate = command.Form.FormRemindDueDate,
                RemindBeforeDays = command.Form.RemindBeforeDays,
                IsSendNotification = command.Form.IsSendNotification,
                PublishToCatalogue = command.Form.PublishToCatalogue
            };

            var formSections = command.FormSections.Select(section => section.BuildFormSection(command.UserId));

            var formQuestions = command.SaveFormQuestionCommands
                .Select(p => p.BuildFormQuestion(p.Id ?? Guid.NewGuid(), form.Id, command.UserId));

            _formBusinessLogicService.EnsureFormValidToSave(form);
            var createdForm = await _formRepository.InsertAsync(form);

            await _formSectionRepository.InsertManyAsync(formSections);
            await _formQuestionRepository.InsertManyAsync(formQuestions);

            await _formUrlExtractor.ExtractFormUrl(form, formQuestions.ToList());

            await _thunderCqrs.SendEvent(new FormChangeEvent(createdForm, FormChangeType.Created), cancellationToken);
        }

        private async Task UpdateForm(SaveFormCommand command, CancellationToken cancellationToken)
        {
            var formQuery = _formRepository
                .GetAllWithAccessControl(AccessControlContext, FormEntityExpressions.HasOwnerOrApprovalPermissionExpr(CurrentUserId))
                .CombineWithAccessRight(_formRepository, _accessRightRepository, CurrentUserId)
                .IgnoreArchivedItems();

            var form = await formQuery.FirstOrDefaultAsync(f => f.Id == command.Form.Id, cancellationToken);
            if (form == null)
            {
                throw new FormAccessDeniedException();
            }

            var currentFormSections = await _formSectionRepository.GetAllListAsync(section => section.FormId == form.Id);
            var currentFormSectionsDic = currentFormSections.ToDictionary(p => p.Id);

            var toUpdatedFormSections = command.FormSections
               .Where(newSection => currentFormSections.Any(currentSection => currentSection.Id == newSection.Id))
               .Select(sectionCommand =>
               {
                   sectionCommand.UpdateExistedFormSection(currentFormSectionsDic[sectionCommand.Id.Value], command.UserId);
                   return currentFormSectionsDic[sectionCommand.Id.Value];
               })
               .ToList();

            var toInsertFormSections = command.FormSections
              .Where(p => !currentFormSections.Any(currentSection => currentSection.Id == p.Id))
              .Select(p => p.BuildFormSection(command.UserId))
              .ToList();

            var toDeleteFormSections = currentFormSections.Where(p => command.ToDeleteFormSectionIds.Contains(p.Id));

            var currentFormQuestions = await _formQuestionRepository.GetAllListAsync(p => p.FormId == form.Id);

            var currentFormQuestionsDic = currentFormQuestions.ToDictionary(p => p.Id);

            var updatedFormQuestions = command.SaveFormQuestionCommands
                .Where(p => currentFormQuestions.Any(currentQuestion => currentQuestion.Id == p.Id))
                .Select(p =>
                {
                    p.UpdateExistedFormQuestion(currentFormQuestionsDic[p.Id.Value], command.UserId);
                    return currentFormQuestionsDic[p.Id.Value];
                })
                .ToList();

            var toInsertFormQuestions = command.SaveFormQuestionCommands
               .Where(p => currentFormQuestions.All(currentQuestion => currentQuestion.Id != p.Id))
               .Select(p => p.BuildFormQuestion(p.Id.Value, form.Id, command.UserId))
               .ToList();

            var toDeleteFormQuestions = currentFormQuestions
                .Where(p => command.ToDeleteFormQuestionIds.Contains(p.Id))
                .ToList();

            var currentFormAnswers = await _formAnswerRepository.GetAllListAsync(p => p.FormId == form.Id);

            var currentFormAnswersDic = currentFormAnswers.ToDictionary(p => p.Id);

            var updatedFormAnswers = currentFormAnswers
                .Select(p =>
                {
                    currentFormAnswersDic[p.Id].PassingStatus = _formAnswerScoreCalculationService
                        .CalcFormAnswerPassingStatus(currentFormAnswersDic[p.Id], form);
                    return currentFormAnswersDic[p.Id];
                })
                .ToList();

            form.Title = command.Form.Title;
            form.Type = command.Form.Type;
            form.SurveyType = command.Form.SurveyType;
            form.IsSurveyTemplate = command.Form.IsSurveyTemplate;
            form.RandomizedQuestions = command.Form.RandomizedQuestions;
            form.MaxAttempt = command.Form.MaxAttempt;
            form.PassingMarkPercentage = command.Form.PassingMarkPercentage;
            form.PassingMarkScore = command.Form.PassingMarkScore;
            form.InSecondTimeLimit = command.Form.InSecondTimeLimit;
            form.PrimaryApprovingOfficerId = command.Form.PrimaryApprovingOfficerId;
            form.AlternativeApprovingOfficerId = command.Form.AlternativeApprovingOfficerId;
            form.IsAllowedDisplayPollResult = command.Form.IsAllowedDisplayPollResult;
            form.OriginalObjectId = form.OriginalObjectId == Guid.Empty ? form.Id : form.OriginalObjectId;
            form.IsShowFreeTextQuestionInPoll = command.Form.IsShowFreeTextQuestionInPoll;
            form.AnswerFeedbackDisplayOption = command.Form.AnswerFeedbackDisplayOption;
            form.AttemptToShowFeedback = command.Form.AttemptToShowFeedback;
            form.SqRatingType = command.Form.SqRatingType;
            form.StartDate = command.Form.StartDate;
            form.EndDate = command.Form.EndDate;
            form.ArchiveDate = command.Form.ArchiveDate;
            form.IsStandalone = command.Form.IsStandalone;
            form.StandaloneMode = command.Form.StandaloneMode;
            form.FormRemindDueDate = command.Form.FormRemindDueDate;
            form.RemindBeforeDays = command.Form.RemindBeforeDays;
            form.IsSendNotification = command.Form.IsSendNotification;
            form.PublishToCatalogue = command.Form.PublishToCatalogue;

            _formBusinessLogicService.EnsureFormValidToSave(form);
            await _formRepository.UpdateAsync(form);

            if (!command.IsAutoSave && command.Form.Status != FormStatus.ReadyToUse)
            {
                _formBusinessLogicService.EnsureFormQuestionsValidToSave(
                    form,
                    currentFormQuestions.Concat(toInsertFormQuestions).Where(p => !command.ToDeleteFormQuestionIds.Contains(p.Id)),
                    currentFormSections.Concat(toInsertFormSections).Where(p => !command.ToDeleteFormSectionIds.Contains(p.Id)));

                var versionTracking = new CreateVersionTrackingParameter
                {
                    VersionSchemaType = VersionSchemaType.Form,
                    ObjectId = form.Id,
                    UserId = command.UserId,
                    ActionComment = "Edited",
                    RevertObjectId = Guid.Empty,
                    CanRollback = false,
                    IncreaseMajorVersion = false,
                    IncreaseMinorVersion = true,
                };

                await _versionTrackingApplicationService.CreateVersionTracking(versionTracking);
            }

            await _formUrlExtractor.ExtractFormUrl(form, currentFormQuestions);

            await _formSectionRepository.DeleteManyAsync(toDeleteFormSections);
            await _formSectionRepository.UpdateManyAsync(toUpdatedFormSections);
            await _formSectionRepository.InsertManyAsync(toInsertFormSections);
            await _formQuestionRepository.DeleteManyAsync(toDeleteFormQuestions);
            await _formQuestionRepository.UpdateManyAsync(updatedFormQuestions);
            await _formQuestionRepository.InsertManyAsync(toInsertFormQuestions);
            await _formAnswerRepository.UpdateManyAsync(updatedFormAnswers);

            await _thunderCqrs.SendEvent(new FormChangeEvent(form, FormChangeType.Updated), cancellationToken);
        }
    }
}
