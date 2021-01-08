using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Conexus.Opal.AccessControl.Infrastructure;
using Microservice.Form.Application.Queries;
using Microservice.Form.Common.Helpers;
using Microservice.Form.Domain.ValueObjects.Questions;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;
using Thunder.Platform.Cqrs;

namespace Microservice.Form.Application.Commands
{
    public class CloneFormAssessmentAsNewVersionCommandHandler : BaseCommandHandler<CloneFormAssessmentAsNewVersionCommand>
    {
        private readonly IThunderCqrs _thunderCqrs;

        public CloneFormAssessmentAsNewVersionCommandHandler(
            IThunderCqrs thunderCqrs,
            IAccessControlContext accessControlContext,
            IUnitOfWorkManager unitOfWorkManager) : base(unitOfWorkManager, accessControlContext)
        {
            _thunderCqrs = thunderCqrs;
        }

        protected override async Task HandleAsync(CloneFormAssessmentAsNewVersionCommand command, CancellationToken cancellationToken)
        {
            var originalForm = await _thunderCqrs.SendQuery(
                new GetFormWithQuestionsByIdQuery
                {
                    FormId = command.Id,
                    UserId = command.UserId
                },
                cancellationToken);
            var clonedWithNewIdForm = F.DeepClone(originalForm)
                .Pipe(_ =>
                {
                    _.Form.Id = command.NewId;
                    _.Form.Status = command.Status;
                    _.Form.Title = command.NewTitle;

                    // Reset versioning fields
                    _.Form.ParentId = _.Form.Id;
                    _.Form.OriginalObjectId = _.Form.OriginalObjectId == Guid.Empty ? _.Form.ParentId : _.Form.OriginalObjectId;

                    if (command.IsCloneToNewVersion == false)
                    {
                        _.Form.OwnerId = command.UserId;
                        _.Form.OriginalObjectId = _.Form.Id;
                    }

                    // Clone form questions
                    var scaleQuestions = _.FormQuestions
                        .Where(question => question.QuestionType == QuestionType.Scale)
                        .Select(question =>
                        {
                            var newId = Guid.NewGuid();
                            question.Id = newId;
                            question.FormId = _.Form.Id;
                            foreach (var opt in question.QuestionOptions)
                            {
                                opt.ScaleId = newId;
                            }

                            return question;
                        }).ToList();
                    var criteriaQuestions = _.FormQuestions
                        .Where(question => question.QuestionType == QuestionType.Criteria)
                        .Select(question =>
                        {
                            question.Id = Guid.NewGuid();
                            question.FormId = _.Form.Id;
                            return question;
                        }).ToList();
                    _.FormQuestions = scaleQuestions.Concat(criteriaQuestions);
                    return _;
                });

            var createFormCommand = new SaveFormCommand
            {
                IsCreation = true,
                Form = clonedWithNewIdForm.Form,
                DepartmentId = originalForm.Form.DepartmentId,
                SaveFormQuestionCommands = clonedWithNewIdForm.FormQuestions
                    .Select(p => new SaveFormQuestionCommand
                    {
                        Id = p.Id == Guid.NewGuid() ? Guid.NewGuid() : p.Id,
                        FormId = p.FormId,
                        QuestionTitle = p.QuestionTitle,
                        QuestionType = p.QuestionType,
                        QuestionCorrectAnswer = p.QuestionCorrectAnswer,
                        QuestionOptions = p.QuestionOptions,
                        Priority = p.Priority,
                        MinorPriority = p.MinorPriority,
                        QuestionHint = p.QuestionHint,
                        AnswerExplanatoryNote = p.AnswerExplanatoryNote,
                        FeedbackCorrectAnswer = p.FeedbackCorrectAnswer,
                        FeedbackWrongAnswer = p.FeedbackWrongAnswer,
                        QuestionLevel = p.QuestionLevel,
                        RandomizedOptions = p.RandomizedOptions,
                        Score = p.Score,
                        IsSurveyTemplateQuestion = p.IsSurveyTemplateQuestion,
                        NextQuestionId = p.NextQuestionId,
                        FormSectionId = p.FormSectionId
                    })
                    .ToList(),

                // If clone to new form, keep owner info.
                UserId = command.IsCloneToNewVersion == false ? clonedWithNewIdForm.Form.OwnerId : originalForm.Form.OwnerId
            };
            await _thunderCqrs.SendCommand(createFormCommand, cancellationToken);
        }
    }
}
