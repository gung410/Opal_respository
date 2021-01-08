using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Conexus.Opal.AccessControl.Entities;
using Conexus.Opal.AccessControl.Extensions;
using Conexus.Opal.AccessControl.Infrastructure;
using Conexus.Opal.Connector.RabbitMQ.Contract;
using Microservice.Form.Application.Events;
using Microservice.Form.Application.Events.EventPayloads;
using Microservice.Form.Application.Models;
using Microservice.Form.Application.Services;
using Microservice.Form.Common.Extensions;
using Microservice.Form.Domain.Entities;
using Microservice.Form.Domain.Services;
using Microservice.Form.Domain.ValueObjects.Form;
using Microservice.Form.Infrastructure;
using Microservice.Form.Versioning.Application.Services;
using Microservice.Form.Versioning.Extensions;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;
using Thunder.Platform.Core.Timing;
using Thunder.Platform.Cqrs;
using CommentEntity = Microservice.Form.Domain.Entities.Comment;
using FormEntity = Microservice.Form.Domain.Entities.Form;

namespace Microservice.Form.Application.Commands
{
    public class UpdateFormAndChangeStatusCommandHandler : BaseCommandHandler<UpdateFormAndChangeStatusCommand>
    {
        private readonly IThunderCqrs _thunderCqrs;
        private readonly IRepository<FormEntity> _formRepository;
        private readonly IRepository<FormQuestion> _formQuestionRepository;
        private readonly IRepository<FormParticipant> _formParticipantRepository;
        private readonly IRepository<UserEntity> _userRepository;
        private readonly IRepository<AccessRight> _accessRightRepository;
        private readonly IRepository<FormSection> _formSectionRepository;
        private readonly IRepository<CommentEntity> _commentRepository;
        private readonly IFormBusinessLogicService _formBusinessLogicService;
        private readonly FormNotifyApplicationService _formParticipantNotifyApplicationService;
        private readonly VersionTrackingApplicationService _versionTrackingApplicationService;
        private readonly WebAppLinkBuilder _webAppLinkBuilder;
        private readonly IFormUrlExtractor _formUrlExtractor;

        public UpdateFormAndChangeStatusCommandHandler(
            IThunderCqrs thunderCqrs,
            IRepository<FormEntity> formRepository,
            IRepository<FormQuestion> formQuestionRepository,
            IRepository<FormParticipant> formParticipantRepository,
            IRepository<UserEntity> userRepository,
            IRepository<AccessRight> accessRightRepository,
            IRepository<FormSection> formSectionRepository,
            IRepository<CommentEntity> commentRepository,
            IFormBusinessLogicService formBusinessLogicService,
            IAccessControlContext accessControlContext,
            FormNotifyApplicationService formParticipantNotifyApplicationService,
            VersionTrackingApplicationService versionTrackingApplicationService,
            WebAppLinkBuilder webAppLinkBuilder,
            IFormUrlExtractor formUrlExtractor,
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
            _commentRepository = commentRepository;
        }

        protected override async Task HandleAsync(UpdateFormAndChangeStatusCommand command, CancellationToken cancellationToken)
        {
            var formQuery = _formRepository
                .GetAllWithAccessControl(AccessControlContext, FormEntityExpressions.HasOwnerOrApprovalPermissionExpr(CurrentUserId))
                .CombineWithAccessRight(_formRepository, _accessRightRepository, CurrentUserId)
                .IgnoreArchivedItems();

            var form = await formQuery.FirstOrDefaultAsync(f => f.Id == command.Form.Id, cancellationToken);

            string actionComment = "Edited";
            Guid revertObjectId = Guid.Empty;

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
                        actionComment = "Published";
                        revertObjectId = form.Id;
                        break;
                    case FormStatus.Unpublished:
                        actionComment = "Unpublished";
                        break;
                    case FormStatus.Approved:
                        actionComment = "Approved";
                        break;
                    case FormStatus.Rejected:
                        actionComment = "Rejected";
                        break;
                    case FormStatus.PendingApproval:
                        form.SubmitDate = Clock.Now;
                        actionComment = "Submited for approval";
                        break;
                    case FormStatus.Draft:
                        actionComment = "Edited";
                        break;
                    case FormStatus.ReadyToUse:
                        actionComment = "Readied for use";
                        break;
                    case FormStatus.Archived:
                        actionComment = "Archived";
                        form.ArchivedBy = CurrentUserId;
                        break;
                    default:
                        actionComment = $"Status changed to {command.Form.Status}";
                        break;
                }
            }

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
            form.StandaloneMode = command.Form.StandaloneMode;
            form.IsStandalone = command.Form.IsStandalone;
            form.FormRemindDueDate = command.Form.FormRemindDueDate;
            form.RemindBeforeDays = command.Form.RemindBeforeDays;
            form.IsSendNotification = command.Form.IsSendNotification;
            form.PublishToCatalogue = command.Form.PublishToCatalogue;

            await _formUrlExtractor.ExtractFormUrl(form, currentFormQuestions);

            _formBusinessLogicService.EnsureFormValidToSave(form);
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

            if (command.Form.Status == FormStatus.Published && form.IsStandalone == true)
            {
                await PerformNotifyAssignedParticipant(form);
            }

            if (command.Form.Status == FormStatus.PendingApproval ||
                command.Form.Status == FormStatus.Approved ||
                command.Form.Status == FormStatus.Rejected)
            {
                await PerformSendNotifyFormIsSubmitted(form, command.Form.Status, command.Comment, cancellationToken);
            }

            await _thunderCqrs.SendEvent(new FormChangeEvent(form, FormChangeType.Updated), cancellationToken);
        }

        private async Task PerformNotifyAssignedParticipant(FormEntity form)
        {
            var formOwnerName = _userRepository.FirstOrDefault(p => p.Id == form.OwnerId).FullName();

            var participants = await _formParticipantRepository
                    .GetAll()
                    .Where(p => p.FormOriginalObjectId == form.OriginalObjectId)
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

        private async Task PerformSendNotifyFormIsSubmitted(FormEntity form, FormStatus commandStatus, string comment, CancellationToken cancellationToken)
        {
            var ownerName = _userRepository.FirstOrDefault(user => user.Id == form.OwnerId).FullName();

            var formUrl = _webAppLinkBuilder.GetFormDetailLink(form.Id);
            string subject = null;
            string template = null;

            var payload = new FormNotifySubmittedRequestPayload
            {
                RecipientName = ownerName,
                FormDetailUrl = formUrl,
                FormName = form.Title,
                FormType = form.Type.ToString(),
                CreatorName = ownerName,
                Comment = comment
            };

            switch (commandStatus)
            {
                case FormStatus.Approved:
                    subject = $"OPAL2.0 - {form.Type} Approved";
                    template = "FormApprovedByAO";
                    payload.RecipientName = ownerName;
                    payload.AOName = _userRepository.FirstOrDefault(user => user.Id == CurrentUserId).FullName();
                    await SendNotifySubmitedEvent(subject, template, payload, form.Id, form.OwnerId, cancellationToken);
                    break;
                case FormStatus.Rejected:
                    subject = $"OPAL2.0 - {form.Type} Rejected";
                    template = "FormRejectedByAO";
                    payload.RecipientName = ownerName;
                    payload.AOName = _userRepository.FirstOrDefault(user => user.Id == CurrentUserId).FullName();
                    await SendNotifySubmitedEvent(subject, template, payload, form.Id, form.OwnerId, cancellationToken);
                    break;
                case FormStatus.PendingApproval:
                    subject = $"OPAL2.0 - New {form.Type} pending approval";
                    template = "FormRequestApproval";
                    if (form.PrimaryApprovingOfficerId.HasValue)
                    {
                        payload.AOName = _userRepository.FirstOrDefault(user => user.Id == form.PrimaryApprovingOfficerId.Value).FullName();
                        payload.RecipientName = payload.AOName;
                        await SendNotifySubmitedEvent(subject, template, payload, form.Id, form.PrimaryApprovingOfficerId.Value, cancellationToken);
                    }

                    if (form.AlternativeApprovingOfficerId.HasValue)
                    {
                        payload.AOName = _userRepository.FirstOrDefault(user => user.Id == form.AlternativeApprovingOfficerId.Value).FullName();
                        payload.RecipientName = payload.AOName;
                        await SendNotifySubmitedEvent(subject, template, payload, form.Id, form.AlternativeApprovingOfficerId.Value, cancellationToken);
                    }

                    break;
                default:
                    break;
            }
        }

        private async Task SendNotifySubmitedEvent(string subject, string template, FormNotifySubmittedRequestPayload payload, Guid formId, Guid receiverId, CancellationToken cancellationToken)
        {
            await _thunderCqrs.SendEvent(
               new FormNotifySubmittedRequestEvent(
                    payload,
                    new ReminderByDto
                    {
                        Type = ReminderByType.AbsoluteDateTimeUTC,
                        Value = Clock.Now.AddMinutes(2).ToString("MM/dd/yyyy HH:mm:ss")
                    },
                    subject,
                    template,
                    formId,
                    receiverId),
               cancellationToken);
        }
    }
}
