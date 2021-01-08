using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Conexus.Opal.AccessControl.Extensions;
using Conexus.Opal.AccessControl.Infrastructure;
using Microservice.Form.Application.Events;
using Microservice.Form.Application.Models;
using Microservice.Form.Common.Extensions;
using Microservice.Form.Domain.Entities;
using Microservice.Form.Domain.Services;
using Microservice.Form.Domain.ValueObjects.Form;
using Microservice.Form.Infrastructure;
using Microservice.Form.Versioning.Extensions;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;
using Thunder.Platform.Core.Exceptions;
using Thunder.Platform.Core.Extensions;
using Thunder.Platform.Core.Timing;
using Thunder.Platform.Cqrs;
using FormEntity = Microservice.Form.Domain.Entities.Form;

namespace Microservice.Form.Application.Commands
{
    public class SaveFormAnswerCommandHandler : BaseCommandHandler<SaveFormAnswerCommand>
    {
        private readonly IRepository<FormEntity> _formRepository;
        private readonly IRepository<FormQuestion> _formQuestionRepository;
        private readonly IRepository<FormAnswer> _formAnswerRepository;
        private readonly IRepository<FormQuestionAnswer> _formQuestionAnswerRepository;
        private readonly IRepository<FormAnswerAttachment> _formAnswerAttachmentsRepository;
        private readonly IRepository<AccessRight> _accessRightRepository;
        private readonly IThunderCqrs _thunderCqrs;
        private readonly IFormAnswerScoreCalculationService _formAnswerScoreCalculationService;

        public SaveFormAnswerCommandHandler(
            IRepository<FormEntity> formRepository,
            IRepository<FormQuestion> formQuestionRepository,
            IRepository<FormAnswer> formAnswerRepository,
            IRepository<FormQuestionAnswer> formQuestionAnswerRepository,
            IRepository<FormAnswerAttachment> formAnswerAttachmentsRepository,
            IRepository<AccessRight> accessRightRepository,
            IThunderCqrs thunderCqrs,
            IFormAnswerScoreCalculationService formAnswerScoreCalculationService,
            IAccessControlContext accessControlContext,
            IUnitOfWorkManager unitOfWorkManager) : base(unitOfWorkManager, accessControlContext)
        {
            _formRepository = formRepository;
            _formQuestionRepository = formQuestionRepository;
            _formAnswerRepository = formAnswerRepository;
            _formQuestionAnswerRepository = formQuestionAnswerRepository;
            _formAnswerAttachmentsRepository = formAnswerAttachmentsRepository;
            _formAnswerScoreCalculationService = formAnswerScoreCalculationService;
            _accessRightRepository = accessRightRepository;
            _thunderCqrs = thunderCqrs;
        }

        protected override async Task HandleAsync(SaveFormAnswerCommand command, CancellationToken cancellationToken)
        {
            if (command.IsCreation)
            {
                await CreateNewFormAnswer(command, cancellationToken);
                return;
            }

            if (command.IsMarking)
            {
                await UpdateFormAnswerScoring(command, cancellationToken);
                return;
            }

            await UpdateFormAnswer(command, cancellationToken);
        }

        private async Task UpdateFormAnswerScoring(SaveFormAnswerCommand command, CancellationToken cancellationToken)
        {
            var formAnswer = await GetFormAnswerAsync(command.FormAnswerId, command.MyCourseId, cancellationToken);

            var form = await GetFormAsync(formAnswer.FormId, cancellationToken);

            var formQuestions = await GetFormQuestionsAsync(formAnswer.FormId, cancellationToken);

            var currentAllQuestionAnswers = await GetFormQuestionAnswersAsync(command.FormAnswerId, cancellationToken);

            var formQuestionsDic = formQuestions.ToDictionary(p => p.Id);

            (currentAllQuestionAnswers, _) = await SaveFormQuestionAnswersAsync(
                command.UserId,
                command.FormAnswerId,
                currentAllQuestionAnswers,
                command.UpdateFormAnswerInfo.QuestionAnswers,
                formQuestionsDic);

            await SaveFormAsnwerEntityAsync(
                command.UserId,
                false,
                form,
                formAnswer,
                currentAllQuestionAnswers,
                command.UpdateFormAnswerInfo.QuestionAnswers,
                formQuestions);

            await _thunderCqrs.SendEvent(new FormSubmitEvent(new FormSubmitEventModel(formAnswer, currentAllQuestionAnswers)), cancellationToken);
        }

        private async Task UpdateFormAnswer(SaveFormAnswerCommand command, CancellationToken cancellationToken)
        {
            var formAnswer = await GetFormAnswerAsync(command.FormAnswerId, command.MyCourseId, cancellationToken);

            if (formAnswer.CreatedBy != command.UserId)
            {
                throw new FormAccessDeniedException();
            }

            var form = await _formRepository.GetAsync(formAnswer.FormId);

            if (form == null)
            {
                throw new FormAccessDeniedException();
            }

            if (formAnswer.SubmitDate != null)
            {
                throw new BusinessLogicException("You can not update the answer because it was submitted");
            }

            if (form.InSecondTimeLimit.HasValue && formAnswer.EndDate <= Clock.Now)
            {
                throw new BusinessLogicException("You can not update the answer because it was out of limit time");
            }

            if (form.DueDate.HasValue && form.DueDate <= Clock.Now)
            {
                throw new BusinessLogicException("You can not update the answer because the form due date has passed");
            }

            var formQuestions = await GetFormQuestionsAsync(formAnswer.FormId, cancellationToken);

            var currentAllQuestionAnswers = await GetFormQuestionAnswersAsync(command.FormAnswerId, cancellationToken);

            var formQuestionsDic = formQuestions.ToDictionary(p => p.Id);

            IEnumerable<Guid> toSaveQuestionAnswerFormQuestionIds;

            (currentAllQuestionAnswers, toSaveQuestionAnswerFormQuestionIds) = await SaveFormQuestionAnswersAsync(
                command.UserId,
                command.FormAnswerId,
                currentAllQuestionAnswers,
                command.UpdateFormAnswerInfo.QuestionAnswers,
                formQuestionsDic);

            await SaveFormAsnwerEntityAsync(
                command.UserId,
                command.UpdateFormAnswerInfo.IsSubmit,
                form,
                formAnswer,
                currentAllQuestionAnswers,
                command.UpdateFormAnswerInfo.QuestionAnswers,
                formQuestions);

            await SaveFormAnswerAttachments(
                command.UserId,
                command.UpdateFormAnswerInfo.QuestionAnswers,
                toSaveQuestionAnswerFormQuestionIds);

            await _thunderCqrs.SendEvent(new FormSubmitEvent(new FormSubmitEventModel(formAnswer, currentAllQuestionAnswers)), cancellationToken);
        }

        private async Task CreateNewFormAnswer(SaveFormAnswerCommand command, CancellationToken cancellationToken)
        {
            var form = await _formRepository.GetAsync(command.FormId);

            var existedFormAnswerCount = await _formAnswerRepository
                .GetAll()
                .WhereIf(command.MyCourseId.HasValue, p => p.MyCourseId == command.MyCourseId)
                .CountAsync(p =>
                    p.FormId == command.FormId
                    && p.CourseId == command.CourseId
                    && p.MyCourseId == command.MyCourseId
                    && p.ClassRunId == command.ClassRunId
                    && p.AssignmentId == command.AssignmentId
                    && p.CreatedBy == command.UserId);

            if (form.MaxAttempt.HasValue && form.MaxAttempt > 0 && existedFormAnswerCount >= form.MaxAttempt)
            {
                throw new MaximumAttemptReachedException();
            }

            var formQuestions = await _formQuestionRepository
                .GetAll()
                .Where(p => p.FormId == command.FormId)
                .ToListAsync(cancellationToken);

            var formMetaData = new FormAnswerFormMetaData();
            if (form.RandomizedQuestions)
            {
                formMetaData.QuestionIdOrderList = formQuestions.Select(p => p.Id).Shuffle();
            }

            var questionWithRandomizedOptions = formQuestions
                .Where(p =>
                    p.RandomizedOptions == true
                    && p.Question_Options != null
                    && p.Question_Options.Any())
                .ToList();

            if (questionWithRandomizedOptions.Any())
            {
                formMetaData.FormQuestionOptionsOrderInfoList = questionWithRandomizedOptions
                    .Select(p => new FormQuestionOptionsOrderInfo
                    {
                        FormQuestionId = p.Id,
                        OptionCodeOrderList = p.Question_Options.Shuffle().Select(opt => opt.Code)
                    });
            }

            var now = Clock.Now;
            var formAnswer = new FormAnswer
            {
                Id = command.FormAnswerId,
                FormId = form.Id,
                MyCourseId = command.MyCourseId,
                CourseId = command.CourseId,
                ClassRunId = command.ClassRunId,
                AssignmentId = command.AssignmentId,
                StartDate = now,
                EndDate =
                    form.InSecondTimeLimit.HasValue
                        ? now.AddSeconds(form.InSecondTimeLimit.Value)
                        : (DateTime?)null,
                Attempt = (short)(existedFormAnswerCount + 1),
                FormMetaData = formMetaData,
                CreatedBy = command.UserId
            };

            await _formAnswerRepository.InsertAsync(formAnswer);

            var formQuestionAnswers = formQuestions
                .Select(p => new FormQuestionAnswer
                {
                    Id = Guid.NewGuid(),
                    FormAnswerId = command.FormAnswerId,
                    FormQuestionId = p.Id,
                    MaxScore = p.Score,
                    CreatedBy = command.UserId
                })
                .ToList();

            await _formQuestionAnswerRepository.InsertManyAsync(formQuestionAnswers);
        }

        private async Task<FormEntity> GetFormAsync(Guid formId, CancellationToken cancellationToken)
        {
            var formQuery = _formRepository
                .GetAllWithAccessControl(AccessControlContext, FormEntityExpressions.HasOwnerPermissionExpr(CurrentUserId))
                .CombineWithAccessRight(_formRepository, _accessRightRepository, CurrentUserId);

            var form = await formQuery.FirstOrDefaultAsync(f => f.Id == formId, cancellationToken);
            if (form == null)
            {
                throw new FormAccessDeniedException();
            }

            return form;
        }

        private async Task<FormAnswer> GetFormAnswerAsync(Guid formAnswerId, Guid? myCourseId, CancellationToken cancellationToken)
        {
            var formAnswer = await _formAnswerRepository
                .GetAll()
                .Where(p => p.Id == formAnswerId)
                .WhereIf(myCourseId.HasValue, p => p.MyCourseId == myCourseId)
                .FirstOrDefaultAsync(cancellationToken);

            if (formAnswer == null)
            {
                throw new FormAccessDeniedException();
            }

            return formAnswer;
        }

        private async Task<IEnumerable<FormQuestion>> GetFormQuestionsAsync(Guid formId, CancellationToken cancellationToken)
        {
            var formQuestions = await _formQuestionRepository
                .GetAll()
                .Where(p => p.FormId == formId)
                .ToListAsync(cancellationToken);

            return formQuestions;
        }

        private async Task<IEnumerable<FormQuestionAnswer>> GetFormQuestionAnswersAsync(Guid formAnswerId, CancellationToken cancellationToken)
        {
            var questionAnswers = await _formQuestionAnswerRepository
                .GetAll()
                .Where(p => p.FormAnswerId == formAnswerId)
                .ToListAsync(cancellationToken);

            return questionAnswers;
        }

        private async Task<(IEnumerable<FormQuestionAnswer> updatedQuestionAnswers, IEnumerable<Guid> toSaveQuestionAnswerIds)> SaveFormQuestionAnswersAsync(
            Guid userId,
            Guid formAnswerId,
            IEnumerable<FormQuestionAnswer> formQuestionAnswers,
            IEnumerable<SaveFormAnswerCommand.UpdateInfoQuestionAnswer> questionAnswersUpdateInfo,
            Dictionary<Guid, FormQuestion> formQuestionsDic)
        {
            var now = Clock.Now;
            if (questionAnswersUpdateInfo != null && questionAnswersUpdateInfo.Any())
            {
                formQuestionAnswers.Upsert(
                    questionAnswersUpdateInfo
                        .Select(p => new FormQuestionAnswer
                        {
                            Id = Guid.NewGuid(),
                            FormAnswerId = formAnswerId,
                            FormQuestionId = p.FormQuestionId,
                            AnswerValue = p.AnswerValue,
                            CreatedBy = userId,
                            SubmittedDate = p.IsSubmit ? Clock.Now : (DateTime?)null,
                            SpentTimeInSeconds = p.SpentTimeInSeconds,
                            MaxScore = formQuestionsDic[p.FormQuestionId].Score,
                            Score = p.MarkedScore != null ?
                                _formAnswerScoreCalculationService.CalculateManualQuestionAnswerScore(formQuestionsDic[p.FormQuestionId], p.MarkedScore.Value) :
                                _formAnswerScoreCalculationService.CalculateQuestionAnswerValueScore(formQuestionsDic[p.FormQuestionId], p.AnswerValue),
                            MarkedBy = p.MarkedScore != null ? userId : Guid.Empty
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

                        if (value.MarkedBy != null)
                        {
                            source.MarkedBy = value.MarkedBy;
                        }

                        source.Score = value.Score;
                        source.ChangedBy = userId;
                        source.ChangedDate = now;
                    });
            }

            var toSaveQuestionAnswerFormQuestionIds = questionAnswersUpdateInfo?.Select(p => p.FormQuestionId).ToList();

            var toSaveFormQuestionAnswers = toSaveQuestionAnswerFormQuestionIds != null && toSaveQuestionAnswerFormQuestionIds.Any()
                ? formQuestionAnswers.Where(p => toSaveQuestionAnswerFormQuestionIds.Contains(p.FormQuestionId)).ToList()
                : new List<FormQuestionAnswer>();

            var toUpdateFormQuestionAnswers = toSaveFormQuestionAnswers.Where(p => p.ChangedDate == now).ToList();
            var toInsertFormQuestionAnswers = toSaveFormQuestionAnswers.Where(p => p.ChangedDate == null).ToList();

            await _formQuestionAnswerRepository.UpdateManyAsync(toUpdateFormQuestionAnswers);
            await _formQuestionAnswerRepository.InsertManyAsync(toInsertFormQuestionAnswers);

            return (formQuestionAnswers, toSaveQuestionAnswerFormQuestionIds);
        }

        private async Task SaveFormAsnwerEntityAsync(
            Guid userId,
            bool isSubmit,
            FormEntity form,
            FormAnswer formAnswer,
            IEnumerable<FormQuestionAnswer> formQuestionAnswers,
            IEnumerable<SaveFormAnswerCommand.UpdateInfoQuestionAnswer> questionAnswersUpdateInfo,
            IEnumerable<FormQuestion> formQuestions)
        {
            var now = Clock.Now;
            if (questionAnswersUpdateInfo != null && questionAnswersUpdateInfo.Any())
            {
                formAnswer.Score = _formAnswerScoreCalculationService.CalcFormAnswerScore(form, formQuestions.ToList(), formAnswer, formQuestionAnswers);
                formAnswer.ScorePercentage = _formAnswerScoreCalculationService.CalcScorePercentage(formAnswer.Score.Value, formQuestions.ToList());
                formAnswer.PassingStatus = _formAnswerScoreCalculationService.CalcFormAnswerPassingStatus(formAnswer, form);
                formAnswer.IsCompleted = formQuestionAnswers.Where(p => p.SubmittedDate != null).Select(p => p.FormQuestionId).ContainsAll(formQuestions.Select(p => p.Id).ToArray());
            }

            if (isSubmit && formAnswer.SubmitDate == null)
            {
                formAnswer.SubmitDate = Clock.Now;
            }

            formAnswer.ChangedBy = userId;

            await _formAnswerRepository.UpdateAsync(formAnswer);
        }

        private async Task SaveFormAnswerAttachments(
            Guid userId,
            IEnumerable<SaveFormAnswerCommand.UpdateInfoQuestionAnswer> questionAnswersUpdateInfo,
            IEnumerable<Guid> toSaveQuestionAnswerFormQuestionIds)
        {
            var now = Clock.Now;
            var formAnswerAttachments = questionAnswersUpdateInfo
                .Where(p => toSaveQuestionAnswerFormQuestionIds.Contains(p.FormQuestionId))
                .Where(p => p.FormAnswerAttachments != null)
                .SelectMany(p => p.FormAnswerAttachments)
                .Select(p => new FormAnswerAttachment
                {
                    Id = Guid.NewGuid(),
                    FormQuestionAnswerId = p.Id,
                    FileName = p.FileName,
                    FileType = p.FileType,
                    FileLocation = p.FileLocation,
                    FileSize = p.FileSize,
                    FileExtension = p.FileExtension,
                    CreatedBy = userId,
                    CreatedDate = now,
                })
                .ToList();

            await _formAnswerAttachmentsRepository.InsertManyAsync(formAnswerAttachments);
        }
    }
}
