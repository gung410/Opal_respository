using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Conexus.Opal.AccessControl.Infrastructure;
using Microservice.LnaForm.Application.Commands.SaveFormAnswer;
using Microservice.LnaForm.Application.Events;
using Microservice.LnaForm.Application.Models;
using Microservice.LnaForm.Common.Extensions;
using Microservice.LnaForm.Domain.Entities;
using Microservice.LnaForm.Domain.ValueObjects.Form;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;
using Thunder.Platform.Core.Exceptions;
using Thunder.Platform.Core.Timing;
using Thunder.Platform.Cqrs;
using FormEntity = Microservice.LnaForm.Domain.Entities.Form;

namespace Microservice.LnaForm.Application.Commands
{
    public class SaveFormAnswerCommandHandler : BaseCommandHandler<SaveFormAnswerCommand>
    {
        private readonly IRepository<FormEntity> _formRepository;
        private readonly IRepository<FormQuestion> _formQuestionRepository;
        private readonly IRepository<FormAnswer> _formAnswerRepository;
        private readonly IRepository<FormQuestionAnswer> _formQuestionAnswerRepository;
        private readonly IThunderCqrs _thunderCqrs;

        public SaveFormAnswerCommandHandler(
            IRepository<FormEntity> formRepository,
            IRepository<FormQuestion> formQuestionRepository,
            IRepository<FormAnswer> formAnswerRepository,
            IRepository<FormQuestionAnswer> formQuestionAnswerRepository,
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

        protected override async Task HandleAsync(SaveFormAnswerCommand command, CancellationToken cancellationToken)
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

        private async Task UpdateFormAnswer(SaveFormAnswerCommand command, CancellationToken cancellationToken)
        {
            var formAnswer = await _formAnswerRepository
                .GetAll()
                .Where(p => p.OwnerId == command.UserId)
                .Where(p => p.Id == command.FormAnswerId)
                .FirstOrDefaultAsync(cancellationToken);

            if (formAnswer == null)
            {
                throw new FormAccessDeniedException();
            }

            if (formAnswer.SubmitDate != null)
            {
                throw new BusinessLogicException("You can not update the answer because it was submitted");
            }

            var form = await _formRepository.GetAsync(formAnswer.FormId);

            var formQuestions = await _formQuestionRepository
                .GetAll()
                .Where(p => p.FormId == formAnswer.FormId)
                .ToListAsync(cancellationToken);

            var formQuestionsDic = formQuestions.ToDictionary(p => p.Id);

            if (form.DueDate.HasValue && form.DueDate <= Clock.Now)
            {
                throw new BusinessLogicException("You can not update the answer because the form due date has passed");
            }

            var currentAllQuestionAnswers = await _formQuestionAnswerRepository
                .GetAll()
                .Where(p => p.FormAnswerId == formAnswer.Id)
                .ToListAsync(cancellationToken);

            var now = Clock.Now;
            if (command.UpdateFormAnswerInfo.QuestionAnswers != null && command.UpdateFormAnswerInfo.QuestionAnswers.Any())
            {
                currentAllQuestionAnswers.Upsert(
                    command.UpdateFormAnswerInfo.QuestionAnswers
                        .Select(p => new FormQuestionAnswer
                        {
                            Id = Guid.NewGuid(),
                            FormAnswerId = command.FormAnswerId,
                            FormQuestionId = p.FormQuestionId,
                            AnswerValue = p.AnswerValue,
                            CreatedBy = command.UserId,
                            SubmittedDate = p.IsSubmit ? Clock.Now : (DateTime?)null,
                            SpentTimeInSeconds = p.SpentTimeInSeconds
                        }),
                    p => p.FormQuestionId,
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

                formAnswer.IsCompleted = currentAllQuestionAnswers.Where(p => p.SubmittedDate != null).Select(p => p.FormQuestionId).ContainsAll(formQuestions.Select(p => p.Id).ToArray());
            }

            if (command.UpdateFormAnswerInfo.IsSubmit && formAnswer.SubmitDate == null)
            {
                formAnswer.SubmitDate = Clock.Now;
            }

            formAnswer.ChangedBy = command.UserId;

            await _formAnswerRepository.UpdateAsync(formAnswer);

            var toSaveQuestionAnswerFormQuestionIds = command.UpdateFormAnswerInfo.QuestionAnswers?.Select(p => p.FormQuestionId).ToList();

            var toSaveFormQuestionAnswers = toSaveQuestionAnswerFormQuestionIds != null && toSaveQuestionAnswerFormQuestionIds.Any()
                ? currentAllQuestionAnswers.Where(p => toSaveQuestionAnswerFormQuestionIds.Contains(p.FormQuestionId)).ToList()
                : new List<FormQuestionAnswer>();

            var toUpdateFormQuestionAnswers = toSaveFormQuestionAnswers.Where(p => p.ChangedDate == now).ToList();
            var toInsertFormQuestionAnswers = toSaveFormQuestionAnswers.Where(p => p.ChangedDate == null).ToList();

            await _formQuestionAnswerRepository.UpdateManyAsync(toUpdateFormQuestionAnswers);
            await _formQuestionAnswerRepository.InsertManyAsync(toInsertFormQuestionAnswers);

            await _thunderCqrs.SendEvent(new FormSubmitEvent(new FormSubmitEventModel(formAnswer, currentAllQuestionAnswers)), cancellationToken);
        }

        private async Task CreateNewFormAnswer(SaveFormAnswerCommand command, CancellationToken cancellationToken)
        {
            var form = await _formRepository.GetAsync(command.FormId);

            var existedFormAnswerCount = await _formAnswerRepository
                .GetAll()
                .CountAsync(p =>
                    p.FormId == command.FormId
                    && p.ResourceId == command.ResourceId
                    && p.CreatedBy == command.UserId);

            var formQuestions = await _formQuestionRepository
                .GetAll()
                .Where(p => p.FormId == command.FormId)
                .ToListAsync(cancellationToken);

            var formMetaData = new FormAnswerFormMetaData();

            var now = Clock.Now;
            var formAnswer = new FormAnswer
            {
                Id = command.FormAnswerId,
                FormId = form.Id,
                ResourceId = command.ResourceId,
                StartDate = now,
                EndDate = null, // TODO: (NhonHT) should remove? Previous version depend on inSecondsTimeLimit but has removed.
                Attempt = (short)(existedFormAnswerCount + 1),
                FormMetaData = formMetaData,
                OwnerId = command.UserId,
                CreatedBy = command.UserId
            };

            await _formAnswerRepository.InsertAsync(formAnswer);

            var formQuestionAnswers = formQuestions
                .Select(p => new FormQuestionAnswer
                {
                    Id = Guid.NewGuid(),
                    FormAnswerId = command.FormAnswerId,
                    FormQuestionId = p.Id,
                    CreatedBy = command.UserId
                })
                .ToList();

            await _formQuestionAnswerRepository.InsertManyAsync(formQuestionAnswers);
        }
    }
}
