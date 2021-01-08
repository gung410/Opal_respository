using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Conexus.Opal.AccessControl.Infrastructure;
using Microservice.StandaloneSurvey.Application.Commands.SaveFormAnswer;
using Microservice.StandaloneSurvey.Application.Events;
using Microservice.StandaloneSurvey.Application.Models;
using Microservice.StandaloneSurvey.Common.Extensions;
using Microservice.StandaloneSurvey.Domain.Entities;
using Microservice.StandaloneSurvey.Domain.ValueObjects.Survey;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;
using Thunder.Platform.Core.Exceptions;
using Thunder.Platform.Core.Timing;
using Thunder.Platform.Cqrs;

namespace Microservice.StandaloneSurvey.Application.Commands
{
    public class SaveSurveyAnswerCommandHandler : BaseCommandHandler<SaveSurveyAnswerCommand>
    {
        private readonly IRepository<Domain.Entities.StandaloneSurvey> _formRepository;
        private readonly IRepository<SurveyQuestion> _formQuestionRepository;
        private readonly IRepository<SurveyAnswer> _formAnswerRepository;
        private readonly IRepository<SurveyQuestionAnswer> _formQuestionAnswerRepository;
        private readonly IThunderCqrs _thunderCqrs;

        public SaveSurveyAnswerCommandHandler(
            IRepository<Domain.Entities.StandaloneSurvey> formRepository,
            IRepository<SurveyQuestion> formQuestionRepository,
            IRepository<SurveyAnswer> formAnswerRepository,
            IRepository<SurveyQuestionAnswer> formQuestionAnswerRepository,
            IThunderCqrs thunderCqrs,
            IAccessControlContext accessControlContext,
            IUnitOfWorkManager unitOfWorkManager) : base(unitOfWorkManager, accessControlContext)
        {
            _formRepository = formRepository;
            _formQuestionRepository = formQuestionRepository;
            _formAnswerRepository = formAnswerRepository;
            _formQuestionAnswerRepository = formQuestionAnswerRepository;
            _thunderCqrs = thunderCqrs;
        }

        protected override async Task HandleAsync(SaveSurveyAnswerCommand command, CancellationToken cancellationToken)
        {
            if (command.IsCreation)
            {
                await CreateNewFormAnswer(command, cancellationToken);
            }
            else
            {
                await UpdateFormAnswer(command, cancellationToken);
            }
        }

        private async Task UpdateFormAnswer(SaveSurveyAnswerCommand command, CancellationToken cancellationToken)
        {
            var formAnswer = await _formAnswerRepository
                .GetAll()
                .Where(p => p.OwnerId == command.UserId)
                .Where(p => p.Id == command.SurveyAnswerId)
                .FirstOrDefaultAsync(cancellationToken);

            if (formAnswer == null)
            {
                throw new SurveyAccessDeniedException();
            }

            if (formAnswer.SubmitDate != null)
            {
                throw new BusinessLogicException("You can not update the answer because it was submitted");
            }

            var form = await _formRepository.GetAsync(formAnswer.FormId);

            var formQuestions = await _formQuestionRepository
                .GetAll()
                .Where(p => p.SurveyId == formAnswer.FormId)
                .ToListAsync(cancellationToken);

            var formQuestionsDic = formQuestions.ToDictionary(p => p.Id);

            if (form.DueDate.HasValue && form.DueDate <= Clock.Now)
            {
                throw new BusinessLogicException("You can not update the answer because the form due date has passed");
            }

            var currentAllQuestionAnswers = await _formQuestionAnswerRepository
                .GetAll()
                .Where(p => p.SurveyAnswerId == formAnswer.Id)
                .ToListAsync(cancellationToken);

            var now = Clock.Now;
            if (command.UpdateFormAnswerInfo.QuestionAnswers != null && command.UpdateFormAnswerInfo.QuestionAnswers.Any())
            {
                currentAllQuestionAnswers.Upsert(
                    command.UpdateFormAnswerInfo.QuestionAnswers
                        .Select(p => new SurveyQuestionAnswer
                        {
                            Id = Guid.NewGuid(),
                            SurveyAnswerId = command.SurveyAnswerId,
                            SurveyQuestionId = p.FormQuestionId,
                            AnswerValue = p.AnswerValue,
                            CreatedBy = command.UserId,
                            SubmittedDate = p.IsSubmit ? Clock.Now : (DateTime?)null,
                            SpentTimeInSeconds = p.SpentTimeInSeconds
                        }),
                    p => p.SurveyQuestionId,
                    (source, value) =>
                    {
                        if (value.AnswerValue != null)
                        {
                            source.AnswerValue = value.AnswerValue;
                        }

                        if (value.SpentTimeInSeconds != null)
                        {
                            source.SpentTimeInSeconds = value.SpentTimeInSeconds;
                        }

                        if (value.SubmittedDate != null)
                        {
                            source.SubmittedDate = value.SubmittedDate;
                        }

                        source.ChangedBy = command.UserId;
                        source.ChangedDate = now;
                    });

                formAnswer.IsCompleted = currentAllQuestionAnswers.Where(p => p.SubmittedDate != null).Select(p => p.SurveyQuestionId).ContainsAll(formQuestions.Select(p => p.Id).ToArray());
            }

            if (command.UpdateFormAnswerInfo.IsSubmit && formAnswer.SubmitDate == null)
            {
                formAnswer.SubmitDate = Clock.Now;
            }

            formAnswer.ChangedBy = command.UserId;

            await _formAnswerRepository.UpdateAsync(formAnswer);

            var toSaveQuestionAnswerFormQuestionIds = command.UpdateFormAnswerInfo.QuestionAnswers?.Select(p => p.FormQuestionId).ToList();

            var toSaveFormQuestionAnswers = toSaveQuestionAnswerFormQuestionIds != null && toSaveQuestionAnswerFormQuestionIds.Any()
                ? currentAllQuestionAnswers.Where(p => toSaveQuestionAnswerFormQuestionIds.Contains(p.SurveyQuestionId)).ToList()
                : new List<SurveyQuestionAnswer>();

            var toUpdateFormQuestionAnswers = toSaveFormQuestionAnswers.Where(p => p.ChangedDate == now).ToList();
            var toInsertFormQuestionAnswers = toSaveFormQuestionAnswers.Where(p => p.ChangedDate == null).ToList();

            await _formQuestionAnswerRepository.UpdateManyAsync(toUpdateFormQuestionAnswers);
            await _formQuestionAnswerRepository.InsertManyAsync(toInsertFormQuestionAnswers);

            await _thunderCqrs.SendEvent(new FormSubmitEvent(new FormSubmitEventModel(formAnswer, currentAllQuestionAnswers)), cancellationToken);
        }

        private async Task CreateNewFormAnswer(SaveSurveyAnswerCommand command, CancellationToken cancellationToken)
        {
            var form = await _formRepository.GetAsync(command.SurveyId);

            var existedFormAnswerCount = await _formAnswerRepository
                .GetAll()
                .CountAsync(p =>
                    p.FormId == command.SurveyId
                    && p.ResourceId == command.ResourceId
                    && p.CreatedBy == command.UserId);

            var formQuestions = await _formQuestionRepository
                .GetAll()
                .Where(p => p.SurveyId == command.SurveyId)
                .ToListAsync(cancellationToken);

            var formMetaData = new SurveyAnswerFormMetaData();

            var now = Clock.Now;
            var formAnswer = new SurveyAnswer
            {
                Id = command.SurveyAnswerId,
                FormId = form.Id,
                ResourceId = command.ResourceId,
                StartDate = now,
                EndDate = null, // TODO: (NhonHT) should remove? Previous version depend on inSecondsTimeLimit but has removed.
                Attempt = (short)(existedFormAnswerCount + 1),
                SurveyAnswerFormMetaData = formMetaData,
                OwnerId = command.UserId,
                CreatedBy = command.UserId
            };

            await _formAnswerRepository.InsertAsync(formAnswer);

            var formQuestionAnswers = formQuestions
                .Select(p => new SurveyQuestionAnswer
                {
                    Id = Guid.NewGuid(),
                    SurveyAnswerId = command.SurveyAnswerId,
                    SurveyQuestionId = p.Id,
                    CreatedBy = command.UserId
                })
                .ToList();

            await _formQuestionAnswerRepository.InsertManyAsync(formQuestionAnswers);
        }
    }
}
