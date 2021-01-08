using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Conexus.Opal.AccessControl.Entities;
using Conexus.Opal.AccessControl.Extensions;
using Conexus.Opal.AccessControl.Infrastructure;
using Microservice.LnaForm.Application.Events;
using Microservice.LnaForm.Application.Models;
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
    public class UpdateFormAndChangeStatusCommandHandler : BaseCommandHandler<UpdateFormAndChangeStatusCommand>
    {
        private readonly IThunderCqrs _thunderCqrs;
        private readonly IRepository<FormEntity> _formRepository;
        private readonly IRepository<FormQuestion> _formQuestionRepository;
        private readonly IRepository<FormParticipant> _formParticipantRepository;
        private readonly IRepository<UserEntity> _userRepository;
        private readonly IFormBusinessLogicService _formBusinessLogicService;
        private readonly FormParticipantNotifyApplicationService _formParticipantNotifyApplicationService;
        private readonly VersionTrackingApplicationService _versionTrackingApplicationService;
        private readonly WebAppLinkBuilder _webAppLinkBuilder;
        private readonly IFormUrlExtractor _formUrlExtractor;
        private readonly IRepository<AccessRight> _accessRightRepository;
        private readonly IRepository<FormSection> _formSectionRepository;

        public UpdateFormAndChangeStatusCommandHandler(
            IThunderCqrs thunderCqrs,
            IRepository<FormEntity> formRepository,
            IRepository<FormQuestion> formQuestionRepository,
            IRepository<FormParticipant> formParticipantRepository,
            IRepository<UserEntity> userRepository,
            IRepository<AccessRight> accessRightRepository,
            IFormBusinessLogicService formBusinessLogicService,
            IAccessControlContext accessControlContext,
            FormParticipantNotifyApplicationService formParticipantNotifyApplicationService,
            VersionTrackingApplicationService versionTrackingApplicationService,
            WebAppLinkBuilder webAppLinkBuilder,
            IFormUrlExtractor formUrlExtractor,
            IRepository<FormSection> formSectionRepository,
            IUnitOfWorkManager unitOfWorkManager) : base(unitOfWorkManager, accessControlContext)
        {
            _thunderCqrs = thunderCqrs;
            _formRepository = formRepository;
            _formQuestionRepository = formQuestionRepository;
            _formParticipantRepository = formParticipantRepository;
            _formBusinessLogicService = formBusinessLogicService;
            _versionTrackingApplicationService = versionTrackingApplicationService;
            _formUrlExtractor = formUrlExtractor;
            _accessRightRepository = accessRightRepository;
            _formSectionRepository = formSectionRepository;
            _formParticipantNotifyApplicationService = formParticipantNotifyApplicationService;
            _userRepository = userRepository;
            _webAppLinkBuilder = webAppLinkBuilder;
        }

        protected override async Task HandleAsync(UpdateFormAndChangeStatusCommand command, CancellationToken cancellationToken)
        {
            var formQuery = _formRepository
                .GetAll()
                .ApplyAccessControlEx(AccessControlContext, LnaFormEntityExpressions.HasOwnerPermissionExpr(CurrentUserId))
                .CombineWithAccessRight(_formRepository, _accessRightRepository, CurrentUserId)
                .IgnoreArchivedItems();

            var form = await formQuery.FirstOrDefaultAsync(f => f.Id == command.Form.Id, cancellationToken);

            string actionComment = "Edited";
            Guid revertObjectId = Guid.Empty;
            bool canRollback = false;
            bool increaseMajorVersion = false;
            bool increaseMinorVersion = true;

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

            var toDeleteFormQuestions = currentFormQuestions.Where(p => command.ToDeleteFormQuestionIds.Contains(p.Id));

            // only run statement bellow if status changed
            if (form.Status != command.Form.Status)
            {
                if (command.Form.Status == FormStatus.Unpublished || command.IsUpdateToNewVersion == true)
                {
                    var cloneFormAsNewVersionCmd = new CloneFormAsNewVersionCommand
                    {
                        Id = form.Id,
                        NewId = command.NewFormID,
                        UserId = command.UserId,
                        ParentId = form.Id,
                        Status = command.Form.Status,
                        NewTitle = command.Form.Title
                    };
                    await _thunderCqrs.SendCommand(cloneFormAsNewVersionCmd, cancellationToken);

                    // Mark current active record as archived version
                    MarkFormAsArchivedCommand markFormAsArchivedCommand = new MarkFormAsArchivedCommand()
                    {
                        Id = form.Id,
                        UserId = command.UserId
                    };

                    await _thunderCqrs.SendCommand(markFormAsArchivedCommand, cancellationToken);

                    form = await _formRepository.GetAsync(cloneFormAsNewVersionCmd.NewId);
                }
                else
                {
                    // TODO: Validate status will be changed if neccessary
                    form.Status = command.Form.Status;
                }

                switch (command.Form.Status)
                {
                    case FormStatus.Published:
                        canRollback = true;
                        increaseMajorVersion = true;
                        actionComment = "Published";
                        revertObjectId = form.Id;
                        break;
                    case FormStatus.Unpublished:
                        actionComment = "Unpublished";
                        increaseMinorVersion = false;
                        break;
                    case FormStatus.Draft:
                        actionComment = "Edited";
                        break;
                    case FormStatus.Archived:
                        increaseMajorVersion = false;
                        increaseMinorVersion = true;
                        actionComment = "Archived";
                        form.ArchivedBy = CurrentUserId;
                        break;
                    default:
                        actionComment = $"Status changed to {command.Form.Status}";
                        break;
                }
            }

            form.Title = command.Form.Title;
            form.OriginalObjectId = form.OriginalObjectId == Guid.Empty ? form.Id : form.OriginalObjectId;
            form.SqRatingType = command.Form.SqRatingType;
            form.StartDate = command.Form.StartDate;
            form.EndDate = command.Form.EndDate;
            form.ArchiveDate = command.Form.ArchiveDate;
            form.FormRemindDueDate = command.Form.FormRemindDueDate;
            form.RemindBeforeDays = command.Form.RemindBeforeDays;

            await _formUrlExtractor.ExtractFormUrl(form, currentFormQuestions);

            await _formRepository.UpdateAsync(form);

            if (!command.IsAutoSave)
            {
                _formBusinessLogicService.EnsureFormQuestionsValidToSave(
                    form,
                    currentFormQuestions.Concat(toInsertFormQuestions).Where(p => !command.ToDeleteFormQuestionIds.Contains(p.Id)),
                    currentFormSections.Concat(toInsertFormSections).Where(p => !command.ToDeleteFormSectionIds.Contains(p.Id)));
            }

            await _formSectionRepository.DeleteManyAsync(toDeleteFormSections);
            await _formSectionRepository.UpdateManyAsync(toUpdatedFormSections);
            await _formSectionRepository.InsertManyAsync(toInsertFormSections);
            await _formQuestionRepository.DeleteManyAsync(toDeleteFormQuestions);
            await _formQuestionRepository.UpdateManyAsync(updatedFormQuestions);
            await _formQuestionRepository.InsertManyAsync(toInsertFormQuestions);

            await _versionTrackingApplicationService.CreateVersionTracking(new CreateVersionTrackingParameter()
            {
                VersionSchemaType = VersionSchemaType.LnaForm,
                ObjectId = form.Id,
                UserId = command.UserId,
                ActionComment = actionComment,
                RevertObjectId = revertObjectId,
                CanRollback = canRollback,
                IncreaseMajorVersion = increaseMajorVersion,
                IncreaseMinorVersion = increaseMinorVersion,
            });

            if (command.Form.Status == FormStatus.Published)
            {
                await PerformNotifyAssignedParticipant(form);
            }

            await _thunderCqrs.SendEvent(new FormChangeEvent(form, FormChangeType.Updated), cancellationToken);
        }

        private async Task PerformNotifyAssignedParticipant(FormEntity form)
        {
            var formOwnerName = _userRepository.FirstOrDefault(p => p.Id == form.OwnerId).FullName();

            var participants = await _formParticipantRepository
                    .GetAll()
                    .Where(p => p.FormId == form.OriginalObjectId)
                    .Join(
                        _userRepository.GetAll(),
                        participantInfo => participantInfo.UserId,
                        userInfo => userInfo.Id,
                        (participant, user) => new { participant.UserId, fullName = user.FullName() })
                    .ToListAsync();

            if (participants.Count == 0)
            {
                return;
            }

            foreach (var participant in participants)
            {
                await _formParticipantNotifyApplicationService.NotifyAssignedFormParticipant(
                           new NotifyFormParticipantModel
                           {
                               FormOriginalObjectId = form.OriginalObjectId,
                               FormOwnerName = formOwnerName,
                               FormTitle = form.Title,
                               ParcitipantId = participant.UserId,
                               ParticipantName = participant.fullName
                           });
            }

            var participantIds = participants.Select(x => x.UserId).ToList();
            var formUrl = _webAppLinkBuilder.GetStandaloneFormPlayerLink(form.Id);
            await _thunderCqrs.SendEvent(
                new StandaloneFormPublishedEvent(
                    new StandaloneFormPublishedEventModel(form, participantIds, formUrl)));
        }
    }
}
