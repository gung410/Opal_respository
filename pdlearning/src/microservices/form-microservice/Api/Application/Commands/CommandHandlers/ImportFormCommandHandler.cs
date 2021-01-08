using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Conexus.Opal.AccessControl.Entities;
using Conexus.Opal.AccessControl.Extensions;
using Conexus.Opal.AccessControl.Infrastructure;
using Microservice.Form.Application.Events;
using Microservice.Form.Application.Models;
using Microservice.Form.Application.Services;
using Microservice.Form.Common.Extensions;
using Microservice.Form.Domain.Entities;
using Microservice.Form.Domain.Services;
using Microservice.Form.Domain.ValueObjects.Form;
using Microservice.Form.Domain.ValueObjects.Questions;
using Microservice.Form.Infrastructure;
using Microservice.Form.Versioning.Application.Commands;
using Microservice.Form.Versioning.Application.RequestDtos;
using Microservice.Form.Versioning.Application.Services;
using Microservice.Form.Versioning.Core;
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
    public class ImportFormCommandHandler : BaseCommandHandler<ImportFormCommand>
    {
        private readonly IThunderCqrs _thunderCqrs;
        private readonly IRepository<FormEntity> _formRepository;
        private readonly IRepository<FormQuestion> _formQuestionRepository;
        private readonly IRepository<FormParticipant> _formParticipantRepository;
        private readonly IRepository<UserEntity> _userRepository;
        private readonly IFormBusinessLogicService _formBusinessLogicService;
        private readonly FormNotifyApplicationService _formParticipantNotifyApplicationService;
        private readonly VersionTrackingApplicationService _versionTrackingApplicationService;
        private readonly IFormUrlExtractor _formUrlExtractor;
        private readonly IRepository<AccessRight> _accessRightRepository;
        private readonly IRepository<FormSection> _formSectionRepository;
        private readonly IEnumerable<ICheckoutVersionResolver> _checkoutVersionResolvers;

        public ImportFormCommandHandler(
            IThunderCqrs thunderCqrs,
            IRepository<FormEntity> formRepository,
            IRepository<FormQuestion> formQuestionRepository,
            IRepository<FormParticipant> formParticipantRepository,
            IRepository<UserEntity> userRepository,
            IRepository<AccessRight> accessRightRepository,
            IFormBusinessLogicService formBusinessLogicService,
            IAccessControlContext accessControlContext,
            FormNotifyApplicationService formParticipantNotifyApplicationService,
            VersionTrackingApplicationService versionTrackingApplicationService,
            IFormUrlExtractor formUrlExtractor,
            IRepository<FormSection> formSectionRepository,
            IEnumerable<ICheckoutVersionResolver> checkoutVersionResolvers,
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
            _checkoutVersionResolvers = checkoutVersionResolvers;
        }

        protected override async Task HandleAsync(ImportFormCommand command, CancellationToken cancellationToken)
        {
            foreach (var data in command.FormWithSectionsQuestions)
            {
                var newFormId = Guid.NewGuid();
                var questionList = new List<FormQuestion>();
                var sectionList = new List<FormSection>();

                var form = new FormEntity
                {
                    Id = newFormId,
                    OwnerId = command.UserId,
                    Title = data.Form.Title,
                    Status = data.Form.Status,
                    Type = data.Form.Type,
                    SurveyType = data.Form.SurveyType,
                    RandomizedQuestions = data.Form.RandomizedQuestions,
                    MaxAttempt = data.Form.MaxAttempt,
                    PassingMarkPercentage = data.Form.PassingMarkPercentage,
                    PassingMarkScore = data.Form.PassingMarkScore,
                    InSecondTimeLimit = data.Form.InSecondTimeLimit,
                    CreatedBy = command.UserId,
                    CreatedDate = Clock.Now,
                    PrimaryApprovingOfficerId = data.Form.PrimaryApprovingOfficerId,
                    AlternativeApprovingOfficerId = data.Form.AlternativeApprovingOfficerId,
                    IsAllowedDisplayPollResult = data.Form.IsAllowedDisplayPollResult,
                    ParentId = data.Form.ParentId,
                    OriginalObjectId = newFormId,
                    DepartmentId = command.DepartmentId,
                    IsShowFreeTextQuestionInPoll = data.Form.IsShowFreeTextQuestionInPoll,
                    AnswerFeedbackDisplayOption = data.Form.AnswerFeedbackDisplayOption,
                    AttemptToShowFeedback = data.Form.AttemptToShowFeedback,
                    SqRatingType = data.Form.SqRatingType,
                    StartDate = data.Form.StartDate,
                    EndDate = data.Form.EndDate,
                    IsStandalone = data.Form.IsStandalone,
                    StandaloneMode = data.Form.StandaloneMode,
                    ArchiveDate = data.Form.ArchiveDate,
                    FormRemindDueDate = data.Form.FormRemindDueDate,
                    RemindBeforeDays = data.Form.RemindBeforeDays,
                    IsSendNotification = data.Form.IsSendNotification
                };

                await _formRepository.InsertAsync(form);

                if (data.FormQuestionsSections.FormQuestions.Any())
                {
                    var questions = data.FormQuestionsSections.FormQuestions;

                    for (int index = 0; index < questions.Count; index++)
                    {
                        var question = new FormQuestion
                        {
                            FormId = newFormId,
                            Title = questions[index].QuestionTitle,
                            Question_Title = questions[index].QuestionTitle,
                            Description = questions[index].Description,
                            Id = Guid.NewGuid(),
                            CreatedBy = command.UserId,
                            CreatedDate = Clock.Now,
                            Score = questions[index].Score,
                            Priority = questions[index].Priority,
                            Question_Type = questions[index].QuestionType,
                            Question_Hint = questions[index].QuestionHint,
                            Question_AnswerExplanatoryNote = questions[index].AnswerExplanatoryNote,
                            Question_CorrectAnswer = questions[index].QuestionCorrectAnswer,
                            Question_Options =
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
                        var section = new FormSection
                        {
                            FormId = newFormId,
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

                var formVersionResolver = _checkoutVersionResolvers.GetCheckoutVersionResolver(VersionSchemaType.Form);
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
                await _thunderCqrs.SendEvent(new FormChangeEvent(form, FormChangeType.Created), cancellationToken);
            }
        }
    }
}
