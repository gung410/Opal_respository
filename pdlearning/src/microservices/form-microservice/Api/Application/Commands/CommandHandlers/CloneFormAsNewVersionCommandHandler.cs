using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Conexus.Opal.AccessControl.Infrastructure;
using Microservice.Form.Application.Queries;
using Microservice.Form.Common.Helpers;
using Microservice.Form.Domain.ValueObjects.Form;
using Microservice.Form.Domain.ValueObjects.Questions;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;
using Thunder.Platform.Cqrs;

namespace Microservice.Form.Application.Commands
{
    public class CloneFormAsNewVersionCommandHandler : BaseCommandHandler<CloneFormAsNewVersionCommand>
    {
        private readonly IThunderCqrs _thunderCqrs;

        public CloneFormAsNewVersionCommandHandler(
            IThunderCqrs thunderCqrs,
            IAccessControlContext accessControlContext,
            IUnitOfWorkManager unitOfWorkManager) : base(unitOfWorkManager, accessControlContext)
        {
            _thunderCqrs = thunderCqrs;
        }

        protected override async Task HandleAsync(CloneFormAsNewVersionCommand command, CancellationToken cancellationToken)
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

                    _.Form.IsShowFreeTextQuestionInPoll = _.Form.IsShowFreeTextQuestionInPoll;

                    _.FormSections = _.FormSections.Select(section =>
                    {
                        section.Id = Guid.NewGuid();
                        section.FormId = _.Form.Id;
                        return section;
                    }).ToList();

                    var sectionsDicByPriority = _.FormSections.ToDictionary(section => section.Priority);

                    // Clone form questions
                    _.FormQuestions = _.FormQuestions
                        .Where(question => question.QuestionType != QuestionType.Smatrix
                             && question.QuestionType != QuestionType.Section
                             && question.QuestionType != QuestionType.Note
                             && question.QuestionType != QuestionType.Qset)
                        .Select(question =>
                        {
                            question.Id = Guid.NewGuid();
                            question.FormId = _.Form.Id;
                            question.FormSectionId = question.FormSectionId.HasValue ?
                                                sectionsDicByPriority[question.Priority].Id
                                              : question.FormSectionId;
                            return question;
                        }).ToList();

                    if (_.Form.Type == FormType.Survey)
                    {
                        var priorityDicByOldId = FormHelper.GetPriorityDicById(originalForm.FormQuestions, originalForm.FormSections);
                        var newIdDicByPriority = FormHelper.GetIdDicByPriority(_.FormQuestions, _.FormSections);
                        _.FormQuestions = FormHelper.UpdateClonedNextQuestionId(_.FormQuestions, priorityDicByOldId, newIdDicByPriority);
                        _.FormSections = FormHelper.UpdateClonedNextQuestionId(_.FormSections, priorityDicByOldId, newIdDicByPriority);
                    }

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
                        Description = p.Description,
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

                FormSections = clonedWithNewIdForm
                    .FormSections
                    .Select(section => new SaveFormSectionsCommand
                    {
                        Id = section.Id == Guid.Empty ? Guid.NewGuid() : section.Id,
                        FormId = section.FormId,
                        MainDescription = section.MainDescription,
                        AdditionalDescription = section.AdditionalDescription,
                        Priority = section.Priority,
                        NextQuestionId = section.NextQuestionId
                    }).ToList(),

                // If clone to new form, keep owner info.
                UserId = command.IsCloneToNewVersion == false ? clonedWithNewIdForm.Form.OwnerId : originalForm.Form.OwnerId
            };
            await _thunderCqrs.SendCommand(createFormCommand, cancellationToken);
        }
    }
}
