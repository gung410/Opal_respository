using System.Collections.Generic;
using Microservice.Course.Common.Helpers;
using Microservice.Course.Domain.Enums;
using Microservice.Course.Domain.ValueObjects;

namespace Microservice.Course.Domain
{
    public static class QuestionCreator
    {
        public static Question NewFreeText(
            string title,
            object correctAnswer = null,
            string hint = null,
            string answerExplanatoryNote = null,
            string feedbackCorrectAnswer = null,
            string feedbackWrongAnswer = null)
        {
            return new Question
            {
                Type = QuestionType.FreeText,
                Title = title,
                CorrectAnswer = correctAnswer,
                Options = null,
                Hint = hint,
                AnswerExplanatoryNote = answerExplanatoryNote,
                FeedbackCorrectAnswer = feedbackCorrectAnswer,
                FeedbackWrongAnswer = feedbackWrongAnswer
            };
        }

        public static Question NewTrueFalse(
            string title,
            bool? correctAnswer = null,
            string hint = null,
            string answerExplanatoryNote = null,
            string feedbackCorrectAnswer = null,
            string feedbackWrongAnswer = null)
        {
            var defaultTrueOption = new QuestionOption(1, true);
            var defaultFalseOption = new QuestionOption(2, false);
            var defaultOptions = F.List(defaultTrueOption, defaultFalseOption);
            return new Question
            {
                Type = QuestionType.TrueFalse,
                Title = title,
                CorrectAnswer = correctAnswer,
                Options = defaultOptions,
                Hint = hint,
                AnswerExplanatoryNote = answerExplanatoryNote,
                FeedbackCorrectAnswer = feedbackCorrectAnswer,
                FeedbackWrongAnswer = feedbackWrongAnswer
            };
        }

        public static Question NewSingleChoice(
            string title,
            IEnumerable<QuestionOption> options,
            object correctAnswer = null,
            string hint = null,
            string answerExplanatoryNote = null)
        {
            return new Question
            {
                Type = QuestionType.SingleChoice,
                Title = title,
                CorrectAnswer = correctAnswer,
                Options = options,
                Hint = hint,
                AnswerExplanatoryNote = answerExplanatoryNote,
                FeedbackCorrectAnswer = null,
                FeedbackWrongAnswer = null
            };
        }

        public static Question NewMultipleChoice(
            string title,
            IEnumerable<QuestionOption> options,
            IEnumerable<object> correctAnswer = null,
            string hint = null,
            string answerExplanatoryNote = null)
        {
            return new Question
            {
                Type = QuestionType.MultipleChoice,
                Title = title,
                CorrectAnswer = correctAnswer,
                Options = options,
                Hint = hint,
                AnswerExplanatoryNote = answerExplanatoryNote,
                FeedbackCorrectAnswer = null,
                FeedbackWrongAnswer = null
            };
        }

        public static Question NewFillInTheBlank(
            string title,
            IEnumerable<QuestionOption> options,
            IEnumerable<object> correctAnswer = null,
            string hint = null,
            string answerExplanatoryNote = null,
            string feedbackCorrectAnswer = null,
            string feedbackWrongAnswer = null)
        {
            return new Question
            {
                Type = QuestionType.FillInTheBlanks,
                Title = title,
                CorrectAnswer = correctAnswer,
                Options = options,
                Hint = hint,
                AnswerExplanatoryNote = answerExplanatoryNote,
                FeedbackCorrectAnswer = feedbackCorrectAnswer,
                FeedbackWrongAnswer = feedbackWrongAnswer
            };
        }

        public static Question NewDropDown(
            string title,
            IEnumerable<QuestionOption> options,
            object correctAnswer = null,
            string hint = null,
            string answerExplanatoryNote = null,
            string feedbackCorrectAnswer = null,
            string feedbackWrongAnswer = null)
        {
            return new Question
            {
                Type = QuestionType.DropDown,
                Title = title,
                CorrectAnswer = correctAnswer,
                Options = options,
                Hint = hint,
                AnswerExplanatoryNote = answerExplanatoryNote,
                FeedbackCorrectAnswer = feedbackCorrectAnswer,
                FeedbackWrongAnswer = feedbackWrongAnswer
            };
        }

        public static Question DatePicker(
            string title,
            IEnumerable<QuestionOption> options,
            object correctAnswer = null,
            string hint = null,
            string answerExplanatoryNote = null,
            string feedbackCorrectAnswer = null,
            string feedbackWrongAnswer = null)
        {
            return new Question
            {
                Type = QuestionType.DatePicker,
                Title = title,
                CorrectAnswer = correctAnswer,
                Options = options,
                Hint = hint,
                AnswerExplanatoryNote = answerExplanatoryNote,
                FeedbackCorrectAnswer = feedbackCorrectAnswer,
                FeedbackWrongAnswer = feedbackWrongAnswer
            };
        }

        public static Question DateRangePicker(
            string title,
            IEnumerable<QuestionOption> options,
            object correctAnswer = null,
            string hint = null,
            string answerExplanatoryNote = null,
            string feedbackCorrectAnswer = null,
            string feedbackWrongAnswer = null)
        {
            return new Question
            {
                Type = QuestionType.DateRangePicker,
                Title = title,
                CorrectAnswer = correctAnswer,
                Options = options,
                Hint = hint,
                AnswerExplanatoryNote = answerExplanatoryNote,
                FeedbackCorrectAnswer = feedbackCorrectAnswer,
                FeedbackWrongAnswer = feedbackWrongAnswer
            };
        }
    }
}
