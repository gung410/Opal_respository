using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Conexus.Opal.AccessControl.Entities;
using Conexus.Opal.AccessControl.Infrastructure;
using Microservice.StandaloneSurvey.Application.Events;
using Microservice.StandaloneSurvey.Application.Services;
using Microservice.StandaloneSurvey.Domain.Entities;
using Microservice.StandaloneSurvey.Domain.Services;
using Microservice.StandaloneSurvey.Domain.ValueObjects.Questions;
using Microservice.StandaloneSurvey.Versioning.Application.Commands;
using Microservice.StandaloneSurvey.Versioning.Application.RequestDtos;
using Microservice.StandaloneSurvey.Versioning.Application.Services;
using Microservice.StandaloneSurvey.Versioning.Core;
using Microservice.StandaloneSurvey.Versioning.Entities;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;
using Thunder.Platform.Core.Timing;
using Thunder.Platform.Cqrs;

namespace Microservice.StandaloneSurvey.Application.Commands
{
    public class ImportFormCommandHandler : BaseCommandHandler<ImportFormCommand>
    {
        private readonly IThunderCqrs _thunderCqrs;
        private readonly IRepository<Domain.Entities.StandaloneSurvey> _formRepository;
        private readonly IRepository<SurveyQuestion> _formQuestionRepository;
        private readonly IRepository<SurveyParticipant> _formParticipantRepository;
        private readonly IRepository<SyncedUser> _userRepository;
        private readonly ISurveyBusinessLogicService _surveyBusinessLogicService;
        private readonly SurveyParticipantNotifyApplicationService _surveyParticipantNotifyApplicationService;
        private readonly VersionTrackingApplicationService _versionTrackingApplicationService;
        private readonly ISurveyUrlExtractor _surveyUrlExtractor;
        private readonly IRepository<AccessRight> _accessRightRepository;
        private readonly IRepository<SurveySection> _formSectionRepository;
        private readonly IEnumerable<ICheckoutVersionResolver> _checkoutVersionResolvers;

        public ImportFormCommandHandler(
            IThunderCqrs thunderCqrs,
            IRepository<Domain.Entities.StandaloneSurvey> formRepository,
            IRepository<SurveyQuestion> formQuestionRepository,
            IRepository<SurveyParticipant> formParticipantRepository,
            IRepository<SyncedUser> userRepository,
            IRepository<AccessRight> accessRightRepository,
            ISurveyBusinessLogicService surveyBusinessLogicService,
            IAccessControlContext accessControlContext,
            SurveyParticipantNotifyApplicationService surveyParticipantNotifyApplicationService,
            VersionTrackingApplicationService versionTrackingApplicationService,
            ISurveyUrlExtractor surveyUrlExtractor,
            IRepository<SurveySection> formSectionRepository,
            IEnumerable<ICheckoutVersionResolver> checkoutVersionResolvers,
            IUnitOfWorkManager unitOfWorkManager) : base(unitOfWorkManager, accessControlContext)
        {
            _thunderCqrs = thunderCqrs;
            _formRepository = formRepository;
            _formQuestionRepository = formQuestionRepository;
            _formParticipantRepository = formParticipantRepository;
            _surveyBusinessLogicService = surveyBusinessLogicService;
            _versionTrackingApplicationService = versionTrackingApplicationService;
            _surveyUrlExtractor = surveyUrlExtractor;
            _accessRightRepository = accessRightRepository;
            _formSectionRepository = formSectionRepository;
            _surveyParticipantNotifyApplicationService = surveyParticipantNotifyApplicationService;
            _userRepository = userRepository;
            _checkoutVersionResolvers = checkoutVersionResolvers;
        }

        protected override async Task HandleAsync(ImportFormCommand command, CancellationToken cancellationToken)
        {
            foreach (var data in command.FormWithSectionsQuestions)
            {
                var newFormId = Guid.NewGuid();
                var questionList = new List<SurveyQuestion>();
                var sectionList = new List<SurveySection>();

                var form = new Domain.Entities.StandaloneSurvey
                {
                    Id = newFormId,
                    OwnerId = command.UserId,
                    Title = data.Form.Title,
                    Status = data.Form.Status,
                    CreatedBy = command.UserId,
                    CreatedDate = Clock.Now,
                    ParentId = data.Form.ParentId,
                    OriginalObjectId = newFormId,
                    DepartmentId = command.DepartmentId,
                    SqRatingType = data.Form.SqRatingType,
                    StartDate = data.Form.StartDate,
                    EndDate = data.Form.EndDate,
                    ArchiveDate = data.Form.ArchiveDate,
                    FormRemindDueDate = data.Form.FormRemindDueDate,
                    RemindBeforeDays = data.Form.RemindBeforeDays,
                    CommunityId = data.Form.CommunityId
                };

                await _formRepository.InsertAsync(form);

                if (data.FormQuestionsSections.FormQuestions.Any())
                {
                    var questions = data.FormQuestionsSections.FormQuestions;

                    for (int index = 0; index < questions.Count; index++)
                    {
                        var question = new SurveyQuestion
                        {
                            SurveyId = newFormId,
                            Title = questions[index].QuestionTitle,
                            Id = Guid.NewGuid(),
                            CreatedBy = command.UserId,
                            CreatedDate = Clock.Now,
                            Priority = questions[index].Priority,
                            QuestionType = questions[index].QuestionType,
                            CorrectAnswer = questions[index].QuestionCorrectAnswer,
                            Options =
                            questions[index].QuestionOptions != null ?
                            questions[index].QuestionOptions.Select(question => (QuestionOption)question) : null
                        };
                        questionList.Add(question);
                    }

                    await _formQuestionRepository.InsertManyAsync(questionList);
                }

                if (data.FormQuestionsSections.FormSections.Any())
                {
                    var sections = data.FormQuestionsSections.FormSections;

                    for (int index = 0; index < sections.Count; index++)
                    {
                        var section = new SurveySection
                        {
                            SurveyId = newFormId,
                            CreatedDate = Clock.Now,
                            Id = Guid.NewGuid(),
                            MainDescription = sections[index].MainDescription,
                            AdditionalDescription = sections[index].AdditionalDescription,
                            CreatedBy = command.UserId,
                            Priority = sections[index].Priority
                        };
                        sectionList.Add(section);
                    }

                    await _formSectionRepository.InsertManyAsync(sectionList);
                }

                var formVersionResolver = _checkoutVersionResolvers.GetCheckoutVersionResolver(VersionSchemaType.StandaloneSurvey);
                var trackingCmd = new SaveVersionTrackingComand()
                {
                    UserId = command.UserId,
                    IsIncreaseMajorVersion = false,
                    IsIncreaseMinorVersion = true,
                    CreationRequest = new CreateVersionTrackingRequest()
                    {
                        VersionId = default,
                        ChangedByUserId = command.UserId,
                        ObjectType = formVersionResolver.GetObjectType(),
                        Data = JsonSerializer.Serialize(form),
                        SchemaVersion = formVersionResolver.GetSchemaVersion(),
                        OriginalObjectId = form.OriginalObjectId == Guid.Empty ? form.Id : form.OriginalObjectId,
                        RevertObjectId = Guid.Empty,
                        CanRollback = false,
                        Comment = $"Created {form.Title}"
                    }
                };

                await _thunderCqrs.SendCommand(trackingCmd);
                await _thunderCqrs.SendEvent(new SurveyChangeEvent(form, SurveyChangeType.Created), cancellationToken);
            }
        }
    }
}
