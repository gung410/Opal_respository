using System;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Conexus.Opal.AccessControl.Extensions;
using Conexus.Opal.AccessControl.Infrastructure;
using Microservice.LnaForm.Application.Events;
using Microservice.LnaForm.Application.Services;
using Microservice.LnaForm.Common.Extensions;
using Microservice.LnaForm.Domain.Entities;
using Microservice.LnaForm.Domain.Services;
using Microservice.LnaForm.Domain.ValueObjects.Form;
using Microservice.LnaForm.Infrastructure;
using Microservice.LnaForm.Versioning.Application.RequestDtos;
using Microservice.LnaForm.Versioning.Application.Services;
using Microservice.LnaForm.Versioning.Entities;
using Microservice.LnaForm.Versioning.Extensions;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;
using Thunder.Platform.Core.Timing;
using Thunder.Platform.Cqrs;
using FormEntity = Microservice.LnaForm.Domain.Entities.Form;

namespace Microservice.LnaForm.Application.Commands
{
    public class SaveFormCommandHandler : BaseCommandHandler<SaveFormCommand>
    {
        private readonly IThunderCqrs _thunderCqrs;
        private readonly IRepository<FormEntity> _formRepository;
        private readonly IRepository<FormQuestion> _formQuestionRepository;
        private readonly IFormBusinessLogicService _formBusinessLogicService;
        private readonly VersionTrackingApplicationService _versionTrackingApplicationService;
        private readonly IFormUrlExtractor _formUrlExtractor;
        private readonly IRepository<AccessRight> _accessRightRepository;
        private readonly IRepository<FormSection> _formSectionRepository;

        public SaveFormCommandHandler(
            IThunderCqrs thunderCqrs,
            IRepository<FormEntity> formRepository,
            IRepository<FormQuestion> formQuestionRepository,
            IFormBusinessLogicService formBusinessLogicService,
            IAccessControlContext accessControlContext,
            IRepository<AccessRight> accessRightRepository,
            VersionTrackingApplicationService versionTrackingApplicationService,
            IFormUrlExtractor formUrlExtractor,
            IRepository<FormSection> formSectionRepository,
            IUnitOfWorkManager unitOfWorkManager) : base(unitOfWorkManager, accessControlContext)
        {
            _thunderCqrs = thunderCqrs;
            _formRepository = formRepository;
            _formQuestionRepository = formQuestionRepository;
            _formBusinessLogicService = formBusinessLogicService;
            _versionTrackingApplicationService = versionTrackingApplicationService;
            _accessRightRepository = accessRightRepository;
            _formUrlExtractor = formUrlExtractor;
            _formSectionRepository = formSectionRepository;
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
                CreatedBy = command.UserId,
                CreatedDate = Clock.Now,
                ParentId = command.Form.ParentId,
                OriginalObjectId = command.Form.OriginalObjectId == Guid.Empty ? command.Form.Id : command.Form.OriginalObjectId,
                DepartmentId = command.DepartmentId,
                SqRatingType = command.Form.SqRatingType,
                StartDate = command.Form.StartDate,
                EndDate = command.Form.EndDate,
                FormRemindDueDate = command.Form.FormRemindDueDate,
                RemindBeforeDays = command.Form.RemindBeforeDays
            };

            var formSections = command.FormSections.Select(section => section.BuildFormSection(command.UserId));

            var formQuestions = command.SaveFormQuestionCommands
                .Select(p => p.BuildFormQuestion(form.Id, command.UserId))
                .ToList();

            var createdForm = await _formRepository.InsertAsync(form);

            await _formSectionRepository.InsertManyAsync(formSections);
            await _formQuestionRepository.InsertManyAsync(formQuestions);

            await _formUrlExtractor.ExtractFormUrl(form, formQuestions);

            await _thunderCqrs.SendEvent(new FormChangeEvent(createdForm, FormChangeType.Created), cancellationToken);
        }

        private async Task UpdateForm(SaveFormCommand command, CancellationToken cancellationToken)
        {
            var formQuery = _formRepository
                .GetAll()
                .ApplyAccessControlEx(AccessControlContext, LnaFormEntityExpressions.HasOwnerPermissionExpr(CurrentUserId))
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
              .Where(p => currentFormSections.All(currentSection => currentSection.Id != p.Id))
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

            form.Title = command.Form.Title;
            form.OriginalObjectId = form.OriginalObjectId == Guid.Empty ? form.Id : form.OriginalObjectId;
            form.SqRatingType = command.Form.SqRatingType;
            form.StartDate = command.Form.StartDate;
            form.EndDate = command.Form.EndDate;
            form.ArchiveDate = command.Form.ArchiveDate;
            form.FormRemindDueDate = command.Form.FormRemindDueDate;
            form.RemindBeforeDays = command.Form.RemindBeforeDays;

            await _formRepository.UpdateAsync(form);

            if (!command.IsAutoSave)
            {
                _formBusinessLogicService.EnsureFormQuestionsValidToSave(
                    form,
                    currentFormQuestions.Concat(toInsertFormQuestions).Where(p => !command.ToDeleteFormQuestionIds.Contains(p.Id)),
                    currentFormSections.Concat(toInsertFormSections).Where(p => !command.ToDeleteFormSectionIds.Contains(p.Id)));

                var versionTracking = new CreateVersionTrackingParameter
                {
                    VersionSchemaType = VersionSchemaType.LnaForm,
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

            await _thunderCqrs.SendEvent(new FormChangeEvent(form, FormChangeType.Updated), cancellationToken);
        }
    }
}
