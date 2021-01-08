using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
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
using Microservice.LnaForm.Domain.ValueObjects.Questions;
using Microservice.LnaForm.Infrastructure;
using Microservice.LnaForm.Versioning.Application.Commands;
using Microservice.LnaForm.Versioning.Application.RequestDtos;
using Microservice.LnaForm.Versioning.Application.Services;
using Microservice.LnaForm.Versioning.Core;
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
    public class ImportFormCommandHandler : BaseCommandHandler<ImportFormCommand>
    {
        private readonly IThunderCqrs _thunderCqrs;
        private readonly IRepository<FormEntity> _formRepository;
        private readonly IRepository<FormQuestion> _formQuestionRepository;
        private readonly IRepository<FormParticipant> _formParticipantRepository;
        private readonly IRepository<UserEntity> _userRepository;
        private readonly IFormBusinessLogicService _formBusinessLogicService;
        private readonly FormParticipantNotifyApplicationService _formParticipantNotifyApplicationService;
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
            FormParticipantNotifyApplicationService formParticipantNotifyApplicationService,
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
                    RemindBeforeDays = data.Form.RemindBeforeDays
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

                var formVersionResolver = _checkoutVersionResolvers.GetCheckoutVersionResolver(VersionSchemaType.LnaForm);
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
