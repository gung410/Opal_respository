using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Conexus.Opal.AccessControl.Infrastructure;
using Microservice.StandaloneSurvey.Application.Events;
using Microservice.StandaloneSurvey.Application.Services;
using Microservice.StandaloneSurvey.Common.Extensions;
using Microservice.StandaloneSurvey.Domain.Entities;
using Microservice.StandaloneSurvey.Domain.Services;
using Microservice.StandaloneSurvey.Domain.ValueObjects;
using Microservice.StandaloneSurvey.Infrastructure;
using Microservice.StandaloneSurvey.Versioning.Application.RequestDtos;
using Microservice.StandaloneSurvey.Versioning.Application.Services;
using Microservice.StandaloneSurvey.Versioning.Entities;
using Microservice.StandaloneSurvey.Versioning.Extensions;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;
using Thunder.Platform.Core.Timing;
using Thunder.Platform.Cqrs;

namespace Microservice.StandaloneSurvey.Application.Commands
{
    public class SaveSurveyCommandHandler : BaseCommandHandler<SaveSurveyCommand>
    {
        private readonly IThunderCqrs _thunderCqrs;
        private readonly IRepository<Domain.Entities.StandaloneSurvey> _formRepository;
        private readonly IRepository<SurveyQuestion> _formQuestionRepository;
        private readonly ISurveyBusinessLogicService _surveyBusinessLogicService;
        private readonly ICslAccessControlContext _cslAccessControlContext;
        private readonly VersionTrackingApplicationService _versionTrackingApplicationService;
        private readonly ISurveyUrlExtractor _surveyUrlExtractor;
        private readonly IRepository<AccessRight> _accessRightRepository;
        private readonly IRepository<SurveySection> _formSectionRepository;

        public SaveSurveyCommandHandler(
            IThunderCqrs thunderCqrs,
            IRepository<Domain.Entities.StandaloneSurvey> formRepository,
            IRepository<SurveyQuestion> formQuestionRepository,
            ISurveyBusinessLogicService surveyBusinessLogicService,
            IAccessControlContext accessControlContext,
            ICslAccessControlContext cslAccessControlContext,
            IRepository<AccessRight> accessRightRepository,
            VersionTrackingApplicationService versionTrackingApplicationService,
            ISurveyUrlExtractor surveyUrlExtractor,
            IRepository<SurveySection> formSectionRepository,
            IUnitOfWorkManager unitOfWorkManager) : base(unitOfWorkManager, accessControlContext)
        {
            _thunderCqrs = thunderCqrs;
            _formRepository = formRepository;
            _formQuestionRepository = formQuestionRepository;
            _surveyBusinessLogicService = surveyBusinessLogicService;
            _cslAccessControlContext = cslAccessControlContext;
            _versionTrackingApplicationService = versionTrackingApplicationService;
            _accessRightRepository = accessRightRepository;
            _surveyUrlExtractor = surveyUrlExtractor;
            _formSectionRepository = formSectionRepository;
        }

        protected override async Task HandleAsync(SaveSurveyCommand command, CancellationToken cancellationToken)
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

        private async Task CreateNewForm(SaveSurveyCommand command, CancellationToken cancellationToken)
        {
            var form = new Domain.Entities.StandaloneSurvey
            {
                Id = command.StandaloneSurvey.Id,
                OwnerId = command.UserId,
                Title = command.StandaloneSurvey.Title,
                Status = command.StandaloneSurvey.Status,
                CreatedBy = command.UserId,
                CreatedDate = Clock.Now,
                ParentId = command.StandaloneSurvey.ParentId,
                OriginalObjectId = command.StandaloneSurvey.OriginalObjectId == Guid.Empty ? command.StandaloneSurvey.Id : command.StandaloneSurvey.OriginalObjectId,
                DepartmentId = command.DepartmentId,
                SqRatingType = command.StandaloneSurvey.SqRatingType,
                StartDate = command.StandaloneSurvey.StartDate,
                EndDate = command.StandaloneSurvey.EndDate,
                FormRemindDueDate = command.StandaloneSurvey.FormRemindDueDate,
                RemindBeforeDays = command.StandaloneSurvey.RemindBeforeDays
            };

            var formSections = command.FormSections.Select(section => section.BuildFormSection(command.UserId));

            var formQuestions = command.SaveFormQuestionCommands
                .Select(p => p.BuildFormQuestion(form.Id, command.UserId))
                .ToList();

            var createdForm = await _formRepository.InsertAsync(form);

            await _formSectionRepository.InsertManyAsync(formSections);
            await _formQuestionRepository.InsertManyAsync(formQuestions);

            await _surveyUrlExtractor.ExtractFormUrl(form, formQuestions);

            await _thunderCqrs.SendEvent(new SurveyChangeEvent(createdForm, SurveyChangeType.Created), cancellationToken);
        }

        private async Task UpdateForm(SaveSurveyCommand command, CancellationToken cancellationToken)
        {
            var formQuery = _formRepository.GetAll();

            if (command.SubModule == SubModule.Lna)
            {
                formQuery = formQuery
                    .ApplyAccessControlEx(
                        AccessControlContext,
                        SurveyEntityExpressions.HasOwnerPermissionExpr(this.CurrentUserId))
                    .CombineWithAccessRight(_formRepository, _accessRightRepository, CurrentUserId)
                    .IgnoreArchivedItems();
            }
            else
            {
                formQuery = formQuery
                    .ApplyCslAccessControl(
                        _cslAccessControlContext,
                        roles: SurveyEntityExpressions.AllManageableCslRoles(),
                        communityId: command.CommunityId,
                        includePredicate: SurveyEntityExpressions.FilterCslSurveyPublishedExpr())
                    .IgnoreArchivedItems();
            }

            var form = await formQuery.FirstOrDefaultAsync(f => f.Id == command.StandaloneSurvey.Id, cancellationToken);
            if (form == null)
            {
                throw new SurveyAccessDeniedException();
            }

            var currentFormSections = await _formSectionRepository.GetAllListAsync(section => section.SurveyId == form.Id);
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
              .Where(p => currentFormSections.All(currentSection => currentSection.Id != p.Id))
              .Select(p => p.BuildFormSection(command.UserId))
              .ToList();

            var toDeleteFormSections = currentFormSections.Where(p => command.ToDeleteFormSectionIds.Contains(p.Id));

            var currentFormQuestions = await _formQuestionRepository.GetAllListAsync(p => p.SurveyId == form.Id);

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

            form.Title = command.StandaloneSurvey.Title;
            form.OriginalObjectId = form.OriginalObjectId == Guid.Empty ? form.Id : form.OriginalObjectId;
            form.SqRatingType = command.StandaloneSurvey.SqRatingType;
            form.StartDate = command.StandaloneSurvey.StartDate;
            form.EndDate = command.StandaloneSurvey.EndDate;
            form.ArchiveDate = command.StandaloneSurvey.ArchiveDate;
            form.FormRemindDueDate = command.StandaloneSurvey.FormRemindDueDate;
            form.RemindBeforeDays = command.StandaloneSurvey.RemindBeforeDays;

            await _formRepository.UpdateAsync(form);

            if (!command.IsAutoSave)
            {
                _surveyBusinessLogicService.EnsureFormQuestionsValidToSave(
                    form,
                    currentFormQuestions.Concat(toInsertFormQuestions).Where(p => !command.ToDeleteFormQuestionIds.Contains(p.Id)),
                    currentFormSections.Concat(toInsertFormSections).Where(p => !command.ToDeleteFormSectionIds.Contains(p.Id)));

                var versionTracking = new CreateVersionTrackingParameter
                {
                    VersionSchemaType = VersionSchemaType.StandaloneSurvey,
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

            await _surveyUrlExtractor.ExtractFormUrl(form, currentFormQuestions);

            await _formSectionRepository.DeleteManyAsync(toDeleteFormSections);
            await _formSectionRepository.UpdateManyAsync(toUpdatedFormSections);
            await _formSectionRepository.InsertManyAsync(toInsertFormSections);
            await _formQuestionRepository.DeleteManyAsync(toDeleteFormQuestions);
            await _formQuestionRepository.UpdateManyAsync(updatedFormQuestions);
            await _formQuestionRepository.InsertManyAsync(toInsertFormQuestions);

            await _thunderCqrs.SendEvent(new SurveyChangeEvent(form, SurveyChangeType.Updated), cancellationToken);
        }
    }
}
