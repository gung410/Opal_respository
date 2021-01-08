using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using Microservice.StandaloneSurvey.Domain.ValueObjects.Questions;
using Thunder.Platform.Core.Domain.Entities;

namespace Microservice.StandaloneSurvey.Domain.Entities
{
    [SuppressMessage("Microsoft.Naming", "CA1724", Justification = "Toan Nguyen confirmed this.")]
    public class SurveyQuestion : BaseEntity, ISoftDelete
    {
        public Guid SurveyId { get; set; }

        public string Title { get; set; }

        public int Priority { get; set; }

        public int? MinorPriority { get; set; }

        /// <summary>
        /// THIS COLUMN IS ONLY USED TO SHOW OPAL1 QUESTIONS DATA IN RELEASE 2.0
        /// WE MAY WILL IMPLEMENT THE APPROPRIATE SOLUTION LATER.
        /// </summary>
        public Guid? ParentId { get; set; }

        public bool IsDeleted { get; set; }

        public Guid? SurveySectionId { get; set; }

        public QuestionType QuestionType { get; set; }

        public object CorrectAnswer { get; set; }

        public IEnumerable<QuestionOption> Options { get; set; }

        public Guid? NextQuestionId { get; set; }

        public Question BuildQuestion()
        {
            return QuestionType switch
            {
                QuestionType.FillInTheBlanks => QuestionCreator.NewFillInTheBlank(
                       Title,
                       Options,
                       CorrectAnswer != null ? JsonSerializer.Deserialize<IEnumerable<object>>(CorrectAnswer.ToString()) : null,
                       NextQuestionId),
                QuestionType.TrueFalse => QuestionCreator.NewTrueFalse(
                        Title,
                        Options,
                        CorrectAnswer != null ? JsonSerializer.Deserialize<bool?>(CorrectAnswer.ToString().ToLower()) : null,
                        NextQuestionId),
                QuestionType.SingleChoice => QuestionCreator.NewSingleChoice(
                        Title,
                        Options,
                        CorrectAnswer,
                        NextQuestionId),
                QuestionType.MultipleChoice => QuestionCreator.NewMultipleChoice(
                        Title,
                        Options,
                        CorrectAnswer != null ? JsonSerializer.Deserialize<IEnumerable<object>>(CorrectAnswer.ToString()) : null,
                        NextQuestionId),
                QuestionType.ShortText => QuestionCreator.NewShortText(
                        Title,
                        CorrectAnswer,
                        NextQuestionId),
                QuestionType.LongText => QuestionCreator.NewLongText(
                        Title,
                        NextQuestionId),
                QuestionType.DropDown => QuestionCreator.NewDropDown(
                        Title,
                        Options,
                        CorrectAnswer,
                        NextQuestionId),
                QuestionType.DatePicker => QuestionCreator.DatePicker(
                        Title,
                        Options,
                        CorrectAnswer,
                        NextQuestionId),
                QuestionType.DateRangePicker => QuestionCreator.DateRangePicker(
                        Title,
                        Options,
                        CorrectAnswer,
                        NextQuestionId),
                _ => null,
            };
        }

        public void SetQuestionContent(Question question)
        {
            QuestionType = question.Type;
            Title = question.Title;
            CorrectAnswer = question.CorrectAnswer;
            Options = question.Options;
        }
    }
}
