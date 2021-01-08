using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Web;
using Microservice.LnaForm.Application.Models;
using Microservice.LnaForm.Domain;
using Microservice.LnaForm.Domain.Entities;
using Microservice.LnaForm.Domain.ValueObjects.Questions;
using Thunder.Platform.Core.Timing;
using Thunder.Platform.Cqrs;

#pragma warning disable SA1402 // File may only contain a single type
namespace Microservice.LnaForm.Application.Commands
{
    public class SaveFormCommand : BaseThunderCommand
    {
        public Guid UserId { get; set; }

        public int DepartmentId { get; set; }

        public FormModel Form { get; set; }

        public IEnumerable<SaveFormQuestionCommand> SaveFormQuestionCommands { get; set; } = new List<SaveFormQuestionCommand>();

        public IEnumerable<SaveFormSectionsCommand> FormSections { get; set; } = new List<SaveFormSectionsCommand>();

        public IEnumerable<Guid> ToDeleteFormSectionIds { get; set; } = new List<Guid>();

        public IEnumerable<Guid> ToDeleteFormQuestionIds { get; set; } = new List<Guid>();

        public bool IsAutoSave { get; set; }

        public bool IsCreation { get; set; }
    }

    public class SaveFormSectionsCommand
    {
        public Guid? Id { get; set; }

        public Guid FormId { get; set; }

        public string MainDescription { get; set; }

        public string AdditionalDescription { get; set; }

        public int Priority { get; set; }

        public Guid? NextQuestionId { get; set; }

        public bool IsDeleted { get; set; }

        public FormSection BuildFormSection(Guid currentUserId)
        {
            return new FormSection
            {
                Id = Id.HasValue ? Id.Value : Guid.NewGuid(),
                FormId = FormId,
                MainDescription = MainDescription,
                AdditionalDescription = AdditionalDescription,
                Priority = Priority,
                NextQuestionId = NextQuestionId,
                IsDeleted = IsDeleted,
                ChangedBy = currentUserId,
                CreatedBy = currentUserId
            };
        }

        public FormSection BuildFormSection(Guid id, Guid currentUserId)
        {
            return new FormSection
            {
                Id = id,
                FormId = FormId,
                MainDescription = MainDescription,
                AdditionalDescription = AdditionalDescription,
                Priority = Priority,
                NextQuestionId = NextQuestionId,
                IsDeleted = IsDeleted,
                ChangedBy = currentUserId,
                CreatedBy = currentUserId
            };
        }

        public FormSection UpdateExistedFormSection(FormSection currentFormSection, Guid changedBy)
        {
            var hasChanged = CopyNormalValuesToFormSection(currentFormSection);
            if (hasChanged)
            {
                currentFormSection.ChangedDate = Clock.Now;
                currentFormSection.ChangedBy = changedBy;
            }

            return currentFormSection;
        }

        /// <summary>
        /// Copy all values to formSection except Id, FormId and Audited Props.
        /// </summary>
        /// <param name="formSection">Target to update value to.</param>
        /// <returns>Return whether has changed or not.</returns>
        private bool CopyNormalValuesToFormSection(FormSection formSection)
        {
            var originalValues = new List<object>()
            {
                formSection.MainDescription,
                formSection.AdditionalDescription,
                formSection.Priority,
                formSection.NextQuestionId
            };

            var updateValues = new List<object>() { MainDescription, AdditionalDescription, Priority };
            if (JsonSerializer.Serialize(originalValues) == JsonSerializer.Serialize(updateValues))
            {
                return false;
            }

            formSection.MainDescription = MainDescription;
            formSection.AdditionalDescription = AdditionalDescription;
            formSection.Priority = Priority;
            formSection.NextQuestionId = NextQuestionId;
            return true;
        }
    }

    public class SaveFormQuestionCommand
    {
        public Guid? Id { get; set; }

        public Guid FormId { get; set; }

        public QuestionType QuestionType { get; set; }

        public string QuestionTitle { get; set; }

        public object QuestionCorrectAnswer { get; set; }

        public IEnumerable<QuestionOptionModel> QuestionOptions { get; set; } = new List<QuestionOptionModel>();

        public int Priority { get; set; }

        public int? MinorPriority { get; set; }

        public Guid? NextQuestionId { get; set; }

        public Guid? FormSectionId { get; set; }

        /// <summary>
        /// Build question from the command.
        /// </summary>
        /// <returns>Question.</returns>
        public Question BuildFormQuestion()
        {
            var questionOptions = QuestionOptions?.Select(p => (QuestionOption)p);
            return QuestionType switch
            {
                QuestionType.FillInTheBlanks => QuestionCreator.NewFillInTheBlank(
                       QuestionTitle,
                       questionOptions,
                       QuestionCorrectAnswer != null ? JsonSerializer.Deserialize<IEnumerable<object>>(QuestionCorrectAnswer.ToString()) : null,
                       NextQuestionId),
                QuestionType.TrueFalse => QuestionCreator.NewTrueFalse(
                        QuestionTitle,
                        questionOptions,
                        QuestionCorrectAnswer != null ? JsonSerializer.Deserialize<bool?>(QuestionCorrectAnswer.ToString().ToLower()) : null,
                        NextQuestionId),
                QuestionType.SingleChoice => QuestionCreator.NewSingleChoice(
                        QuestionTitle,
                        questionOptions,
                        QuestionCorrectAnswer,
                        NextQuestionId),
                QuestionType.MultipleChoice => QuestionCreator.NewMultipleChoice(
                        QuestionTitle,
                        questionOptions,
                        QuestionCorrectAnswer != null ? JsonSerializer.Deserialize<IEnumerable<object>>(QuestionCorrectAnswer.ToString()) : null,
                        NextQuestionId),
                QuestionType.ShortText => QuestionCreator.NewShortText(
                        QuestionTitle,
                        QuestionCorrectAnswer,
                        NextQuestionId),
                QuestionType.LongText => QuestionCreator.NewLongText(
                        QuestionTitle,
                        NextQuestionId),
                QuestionType.DropDown => QuestionCreator.NewDropDown(
                        QuestionTitle,
                        questionOptions,
                        QuestionCorrectAnswer,
                        NextQuestionId),
                QuestionType.DatePicker => QuestionCreator.DatePicker(
                        QuestionTitle,
                        questionOptions,
                        QuestionCorrectAnswer,
                        NextQuestionId),
                QuestionType.DateRangePicker => QuestionCreator.DateRangePicker(
                        QuestionTitle,
                        questionOptions,
                        QuestionCorrectAnswer,
                        NextQuestionId),
                _ => null,
            };
        }

        public FormQuestion BuildFormQuestion(Guid id, Guid formId, Guid createdBy)
        {
            var result = new FormQuestion
            {
                Id = id,
                FormId = formId,
                CreatedDate = Clock.Now,
                CreatedBy = createdBy
            };
            CopyNormalValuesToFormQuestion(result);

            return result;
        }

        public FormQuestion BuildFormQuestion(Guid formId, Guid createdBy)
        {
            var result = new FormQuestion
            {
                Id = Id.HasValue ? Id.Value : Guid.NewGuid(),
                FormId = formId,
                CreatedDate = Clock.Now,
                CreatedBy = createdBy
            };
            CopyNormalValuesToFormQuestion(result);

            return result;
        }

        public FormQuestion UpdateExistedFormQuestion(FormQuestion currentFormQuestion, Guid changedBy)
        {
            var hasChanged = CopyNormalValuesToFormQuestion(currentFormQuestion);
            if (hasChanged)
            {
                currentFormQuestion.ChangedDate = Clock.Now;
                currentFormQuestion.ChangedBy = changedBy;
            }

            return currentFormQuestion;
        }

        /// <summary>
        /// Copy all values to formQuestion except Id, FormId and Audited Props.
        /// </summary>
        /// <param name="formQuestion">Target to update value to.</param>
        /// <returns>Return whether has changed or not.</returns>
        private bool CopyNormalValuesToFormQuestion(FormQuestion formQuestion)
        {
            var originalValues = new List<object>()
            {
                formQuestion.Title,
                formQuestion.Priority,
                formQuestion.MinorPriority,
                formQuestion.FormSectionId,
                formQuestion.BuildQuestion()
            };

            var updateValues = new List<object>()
            {
                QuestionTitle,
                Priority,
                MinorPriority,
                FormSectionId,
                this.BuildFormQuestion()
            };

            if (JsonSerializer.Serialize(originalValues) == JsonSerializer.Serialize(updateValues))
            {
                return false;
            }

            formQuestion.Title = HttpUtility.HtmlDecode(QuestionTitle) != QuestionTitle
                        ? QuestionTitle
                        : HttpUtility.HtmlEncode(QuestionTitle);

            formQuestion.Priority = Priority;
            formQuestion.MinorPriority = MinorPriority;
            formQuestion.NextQuestionId = NextQuestionId;
            formQuestion.FormSectionId = FormSectionId;
            formQuestion.SetQuestionContent(this.BuildFormQuestion());
            return true;
        }
    }
}
#pragma warning restore SA1402 // File may only contain a single type
