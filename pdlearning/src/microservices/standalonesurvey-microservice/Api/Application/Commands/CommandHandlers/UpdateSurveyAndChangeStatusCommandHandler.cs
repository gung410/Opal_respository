using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Conexus.Opal.AccessControl.Entities;
using Conexus.Opal.AccessControl.Infrastructure;
using Microservice.StandaloneSurvey.Application.Events;
using Microservice.StandaloneSurvey.Application.Models;
using Microservice.StandaloneSurvey.Application.Services;
using Microservice.StandaloneSurvey.Common.Extensions;
using Microservice.StandaloneSurvey.Domain.Entities;
using Microservice.StandaloneSurvey.Domain.Services;
using Microservice.StandaloneSurvey.Domain.ValueObjects.Survey;
using Microservice.StandaloneSurvey.Infrastructure;
using Microservice.StandaloneSurvey.Versioning.Application.RequestDtos;
using Microservice.StandaloneSurvey.Versioning.Application.Services;
using Microservice.StandaloneSurvey.Versioning.Entities;
using Microservice.StandaloneSurvey.Versioning.Extensions;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;
using Thunder.Platform.Cqrs;

namespace Microservice.StandaloneSurvey.Application.Commands
{
    public class UpdateSurveyAndChangeStatusCommandHandler : BaseCommandHandler<UpdateSurveyAndChangeStatusCommand>
    {
        private readonly IThunderCqrs _thunderCqrs;
        private readonly IRepository<Domain.Entities.StandaloneSurvey> _formRepository;
        private readonly IRepository<SurveyQuestion> _formQuestionRepository;
        private readonly IRepository<SurveyParticipant> _formParticipantRepository;
        private readonly IRepository<SyncedUser> _userRepository;
        private readonly ISurveyBusinessLogicService _surveyBusinessLogicService;
        private readonly ICslAccessControlContext _cslAccessControlContext;
        private readonly SurveyParticipantNotifyApplicationService _surveyParticipantNotifyApplicationService;
        private readonly VersionTrackingApplicationService _versionTrackingApplicationService;
        private readonly WebAppLinkBuilder _webAppLinkBuilder;
        private readonly ISurveyUrlExtractor _surveyUrlExtractor;
        private readonly IRepository<AccessRight> _accessRightRepository;
        private readonly IRepository<SurveySection> _formSectionRepository;
        private readonly IRepository<SyncedCslCommunityMember> _cslMemberRepository;
        private readonly IRepository<SurveyResponse> _surveyResponseRepository;

        public UpdateSurveyAndChangeStatusCommandHandler(
            IThunderCqrs thunderCqrs,
            IRepository<Domain.Entities.StandaloneSurvey> formRepository,
            IRepository<SurveyQuestion> formQuestionRepository,
            IRepository<SurveyParticipant> formParticipantRepository,
            IRepository<SyncedUser> userRepository,
            IRepository<AccessRight> accessRightRepository,
            ISurveyBusinessLogicService surveyBusinessLogicService,
            ICslAccessControlContext cslAccessControlContext,
            IAccessControlContext accessControlContext,
            SurveyParticipantNotifyApplicationService surveyParticipantNotifyApplicationService,
            VersionTrackingApplicationService versionTrackingApplicationService,
            WebAppLinkBuilder webAppLinkBuilder,
            ISurveyUrlExtractor surveyUrlExtractor,
            IRepository<SurveySection> formSectionRepository,
            IRepository<SyncedCslCommunityMember> cslMemberRepository,
            IRepository<SurveyResponse> surveyResponseRepository,
            IUnitOfWorkManager unitOfWorkManager) : base(unitOfWorkManager, accessControlContext)
        {
            _thunderCqrs = thunderCqrs;
            _formRepository = formRepository;
            _formQuestionRepository = formQuestionRepository;
            _formParticipantRepository = formParticipantRepository;
            _surveyBusinessLogicService = surveyBusinessLogicService;
            _cslAccessControlContext = cslAccessControlContext;
            _versionTrackingApplicationService = versionTrackingApplicationService;
            _surveyUrlExtractor = surveyUrlExtractor;
            _accessRightRepository = accessRightRepository;
            _formSectionRepository = formSectionRepository;
            _cslMemberRepository = cslMemberRepository;
            _surveyResponseRepository = surveyResponseRepository;
            _surveyParticipantNotifyApplicationService = surveyParticipantNotifyApplicationService;
            _userRepository = userRepository;
            _webAppLinkBuilder = webAppLinkBuilder;
        }

        protected override async Task HandleAsync(UpdateSurveyAndChangeStatusCommand command, CancellationToken cancellationToken)
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

            string actionComment = "Edited";
            Guid revertObjectId = Guid.Empty;
            bool canRollback = false;
            bool increaseMajorVersion = false;
            bool increaseMinorVersion = true;

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
              .Where(p => !currentFormSections.Any(currentSection => currentSection.Id == p.Id))
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

            var toDeleteFormQuestions = currentFormQuestions.Where(p => command.ToDeleteFormQuestionIds.Contains(p.Id));

            // only run statement bellow if status changed
            if (form.Status != command.StandaloneSurvey.Status)
            {
                if (command.StandaloneSurvey.Status == SurveyStatus.Unpublished || command.IsUpdateToNewVersion == true)
                {
                    var cloneFormAsNewVersionCmd = new CloneSurveyAsNewVersionCommand
                    {
                        Id = form.Id,
                        NewId = command.NewFormID,
                        UserId = command.UserId,
                        ParentId = form.Id,
                        Status = command.StandaloneSurvey.Status,
                        NewTitle = command.StandaloneSurvey.Title
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
                    form.Status = command.StandaloneSurvey.Status;
                }

                switch (command.StandaloneSurvey.Status)
                {
                    case SurveyStatus.Published:
                        canRollback = true;
                        increaseMajorVersion = true;
                        actionComment = "Published";
                        revertObjectId = form.Id;
                        break;
                    case SurveyStatus.Unpublished:
                        actionComment = "Unpublished";
                        increaseMinorVersion = false;
                        break;
                    case SurveyStatus.Draft:
                        actionComment = "Edited";
                        break;
                    case SurveyStatus.Archived:
                        increaseMajorVersion = false;
                        increaseMinorVersion = true;
                        actionComment = "Archived";
                        form.ArchivedBy = CurrentUserId;
                        break;
                    default:
                        actionComment = $"Status changed to {command.StandaloneSurvey.Status}";
                        break;
                }

                // Only for csl
                await PopulateResponseList(command);
            }

            form.Title = command.StandaloneSurvey.Title;
            form.OriginalObjectId = form.OriginalObjectId == Guid.Empty ? form.Id : form.OriginalObjectId;
            form.SqRatingType = command.StandaloneSurvey.SqRatingType;
            form.StartDate = command.StandaloneSurvey.StartDate;
            form.EndDate = command.StandaloneSurvey.EndDate;
            form.ArchiveDate = command.StandaloneSurvey.ArchiveDate;
            form.FormRemindDueDate = command.StandaloneSurvey.FormRemindDueDate;
            form.RemindBeforeDays = command.StandaloneSurvey.RemindBeforeDays;

            await _surveyUrlExtractor.ExtractFormUrl(form, currentFormQuestions);

            await _formRepository.UpdateAsync(form);

            if (!command.IsAutoSave)
            {
                _surveyBusinessLogicService.EnsureFormQuestionsValidToSave(
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
                VersionSchemaType = VersionSchemaType.StandaloneSurvey,
                ObjectId = form.Id,
                UserId = command.UserId,
                ActionComment = actionComment,
                RevertObjectId = revertObjectId,
                CanRollback = canRollback,
                IncreaseMajorVersion = increaseMajorVersion,
                IncreaseMinorVersion = increaseMinorVersion,
            });

            if (command.StandaloneSurvey.Status == SurveyStatus.Published)
            {
                await PerformNotifyAssignedParticipant(form, command.SubModule);
            }

            await _thunderCqrs.SendEvent(new SurveyChangeEvent(form, SurveyChangeType.Updated), cancellationToken);
        }

        private async Task PopulateResponseList(UpdateSurveyAndChangeStatusCommand command)
        {
            if (command.SubModule == SubModule.Csl && command.StandaloneSurvey.Status == SurveyStatus.Published)
            {
                var members = await _cslMemberRepository
                                    .GetAll()
                                    .Where(_ => _.CommunityId == command.CommunityId)
                                    .ToListAsync();

                var responses = members.Select(_ => new SurveyResponse
                {
                    FormId = command.StandaloneSurvey.Id,
                    Id = Guid.NewGuid(),
                    UserId = _.UserId
                });

                await _surveyResponseRepository.InsertManyAsync(responses);
            }
        }

        private async Task PerformNotifyAssignedParticipant(
            Domain.Entities.StandaloneSurvey form,
            SubModule subModule)
        {
            var formOwnerName = _userRepository.FirstOrDefault(p => p.Id == form.OwnerId).FullName();

            var participants = await _formParticipantRepository
                    .GetAll()
                    .Where(p => p.SurveyId == form.OriginalObjectId)
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
                await _surveyParticipantNotifyApplicationService.NotifyAssignedFormParticipant(
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
            var formUrl = _webAppLinkBuilder.GetStandaloneFormPlayerLink(form.Id, subModule);
            await _thunderCqrs.SendEvent(
                new StandaloneFormPublishedEvent(
                    new StandaloneFormPublishedEventModel(form, participantIds, formUrl)));
        }
    }
}
