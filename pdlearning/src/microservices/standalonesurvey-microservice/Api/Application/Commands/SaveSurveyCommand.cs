using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Web;
using Microservice.StandaloneSurvey.Application.Models;
using Microservice.StandaloneSurvey.Domain;
using Microservice.StandaloneSurvey.Domain.Entities;
using Microservice.StandaloneSurvey.Domain.ValueObjects.Questions;
using Thunder.Platform.Core.Timing;
using Thunder.Platform.Cqrs;

#pragma warning disable SA1402 // File may only contain a single type
namespace Microservice.StandaloneSurvey.Application.Commands
{
    public class SaveSurveyCommand : BaseStandaloneSurveyCommand
    {
        public Guid UserId { get; set; }

        public int DepartmentId { get; set; }

        public StandaloneSurveyModel StandaloneSurvey { get; set; }

        public IEnumerable<SaveSurveyQuestionCommand> SaveFormQuestionCommands { get; set; } = new List<SaveSurveyQuestionCommand>();

        public IEnumerable<SaveSurveySectionsCommand> FormSections { get; set; } = new List<SaveSurveySectionsCommand>();

        public IEnumerable<Guid> ToDeleteFormSectionIds { get; set; } = new List<Guid>();

        public IEnumerable<Guid> ToDeleteFormQuestionIds { get; set; } = new List<Guid>();

        public bool IsAutoSave { get; set; }

        public bool IsCreation { get; set; }
    }

    public class SaveSurveySectionsCommand
    {
        public Guid? Id { get; set; }

        public Guid SurveyId { get; set; }

        public string MainDescription { get; set; }

        public string AdditionalDescription { get; set; }

        public int Priority { get; set; }

        public Guid? NextQuestionId { get; set; }

        public bool IsDeleted { get; set; }

        public SurveySection BuildFormSection(Guid currentUserId)
        {
            return new SurveySection
            {
                Id = Id.HasValue ? Id.Value : Guid.NewGuid(),
                SurveyId = SurveyId,
                MainDescription = MainDescription,
                AdditionalDescription = AdditionalDescription,
                Priority = Priority,
                NextQuestionId = NextQuestionId,
                IsDeleted = IsDeleted,
                ChangedBy = currentUserId,
                CreatedBy = currentUserId
            };
        }

        public SurveySection BuildFormSection(Guid id, Guid currentUserId)
        {
            return new SurveySection
            {
                Id = id,
                SurveyId = SurveyId,
                MainDescription = MainDescription,
                AdditionalDescription = AdditionalDescription,
                Priority = Priority,
                NextQuestionId = NextQuestionId,
                IsDeleted = IsDeleted,
                ChangedBy = currentUserId,
                CreatedBy = currentUserId
            };
        }

        public SurveySection UpdateExistedFormSection(SurveySection currentFormSection, Guid changedBy)
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
        private bool CopyNormalValuesToFormSection(SurveySection formSection)
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

    public class SaveSurveyQuestionCommand
    {
        public Guid? Id { get; set; }

        public Guid SurveyId { get; set; }

        public QuestionType QuestionType { get; set; }

        public string QuestionTitle { get; set; }

        public object QuestionCorrectAnswer { get; set; }

        public IEnumerable<QuestionOptionModel> QuestionOptions { get; set; } = new List<QuestionOptionModel>();

        public int Priority { get; set; }

        public int? MinorPriority { get; set; }

        public Guid? NextQuestionId { get; set; }

        public Guid? SurveySectionId { get; set; }

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

        public SurveyQuestion BuildFormQuestion(Guid id, Guid formId, Guid createdBy)
        {
            var result = new SurveyQuestion
            {
                Id = id,
                SurveyId = formId,
                CreatedDate = Clock.Now,
                CreatedBy = createdBy
            };
            CopyNormalValuesToFormQuestion(result);

            return result;
        }

        public SurveyQuestion BuildFormQuestion(Guid formId, Guid createdBy)
        {
            var result = new SurveyQuestion
            {
                Id = Id.HasValue ? Id.Value : Guid.NewGuid(),
                SurveyId = formId,
                CreatedDate = Clock.Now,
                CreatedBy = createdBy
            };
            CopyNormalValuesToFormQuestion(result);

            return result;
        }

        public SurveyQuestion UpdateExistedFormQuestion(SurveyQuestion currentFormQuestion, Guid changedBy)
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
        private bool CopyNormalValuesToFormQuestion(SurveyQuestion formQuestion)
        {
            var originalValues = new List<object>()
            {
                formQuestion.Title,
                formQuestion.Priority,
                formQuestion.MinorPriority,
                formQuestion.SurveySectionId,
                formQuestion.BuildQuestion()
            };

            var updateValues = new List<object>()
            {
                QuestionTitle,
                Priority,
                MinorPriority,
                SurveySectionId,
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
            formQuestion.SurveySectionId = SurveySectionId;
            formQuestion.SetQuestionContent(this.BuildFormQuestion());
            return true;
        }
    }
}
#pragma warning restore SA1402 // File may only contain a single type
