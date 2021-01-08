using System;
using System.Collections.Generic;
using Microservice.StandaloneSurvey.Domain.ValueObjects.Questions;

namespace Microservice.StandaloneSurvey.Domain
{
    public static class QuestionCreator
    {
        public static Question NewShortText(
            string title,
            object correctAnswer = null,
            Guid? nextQuestionId = null)
        {
            return new Question
            {
                Type = QuestionType.ShortText,
                Title = title,
                CorrectAnswer = correctAnswer,
                Options = null,
                NextQuestionId = nextQuestionId
            };
    }

        public static Question NewLongText(
            string title,
            Guid? nextQuestionId = null)
        {
            return new Question
            {
                Type = QuestionType.LongText,
                Title = title,
                CorrectAnswer = null,
                Options = null,
                NextQuestionId = nextQuestionId
            };
    }

        public static Question NewTrueFalse(
            string title,
            IEnumerable<QuestionOption> options = null,
            bool? correctAnswer = null,
            Guid? nextQuestionId = null)
        {
            var defaultTrueOption = new QuestionOption(1, true, null, null, null, nextQuestionId);
            var defaultFalseOption = new QuestionOption(2, false, null, null, null, nextQuestionId);
            var defaultOptions = new List<QuestionOption>() { defaultTrueOption, defaultFalseOption };
            return new Question
            {
                Type = QuestionType.TrueFalse,
                Title = title,
                CorrectAnswer = correctAnswer,
                Options = options ?? defaultOptions,
                NextQuestionId = nextQuestionId
            };
    }

        public static Question NewSingleChoice(
            string title,
            IEnumerable<QuestionOption> options,
            object correctAnswer = null,
            Guid? nextQuestionId = null)
        {
            return new Question
            {
                Type = QuestionType.SingleChoice,
                Title = title,
                CorrectAnswer = correctAnswer,
                Options = options,
                NextQuestionId = nextQuestionId
            };
    }

        public static Question NewMultipleChoice(
            string title,
            IEnumerable<QuestionOption> options,
            IEnumerable<object> correctAnswer = null,
            Guid? nextQuestionId = null)
        {
            return new Question
            {
                Type = QuestionType.MultipleChoice,
                Title = title,
                CorrectAnswer = correctAnswer,
                Options = options,
                NextQuestionId = nextQuestionId
            };
    }

        public static Question NewFillInTheBlank(
            string title,
            IEnumerable<QuestionOption> options,
            IEnumerable<object> correctAnswer = null,
            Guid? nextQuestionId = null)
        {
            return new Question
            {
                Type = QuestionType.FillInTheBlanks,
                Title = title,
                CorrectAnswer = correctAnswer,
                Options = options,
                NextQuestionId = nextQuestionId
            };
    }

        public static Question NewDropDown(
            string title,
            IEnumerable<QuestionOption> options,
            object correctAnswer = null,
            Guid? nextQuestionId = null)
        {
            return new Question
            {
                Type = QuestionType.DropDown,
                Title = title,
                CorrectAnswer = correctAnswer,
                Options = options,
                NextQuestionId = nextQuestionId
            };
    }

        public static Question DatePicker(
            string title,
            IEnumerable<QuestionOption> options,
            object correctAnswer = null,
            Guid? nextQuestionId = null)
        {
            return new Question
            {
                Type = QuestionType.DatePicker,
                Title = title,
                CorrectAnswer = correctAnswer,
                Options = options,
                NextQuestionId = nextQuestionId
            };
    }

        public static Question DateRangePicker(
            string title,
            IEnumerable<QuestionOption> options,
            object correctAnswer = null,
            Guid? nextQuestionId = null)
        {
            return new Question
            {
                Type = QuestionType.DateRangePicker,
                Title = title,
                CorrectAnswer = correctAnswer,
                Options = options,
                NextQuestionId = nextQuestionId
            };
        }
    }
}
