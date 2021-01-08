using System;
using System.Collections.Generic;
using Microservice.Form.Domain.ValueObjects.Questions;

namespace Microservice.Form.Domain
{
    public static class QuestionCreator
    {
        public static Question NewShortText(
            string title,
            object correctAnswer = null,
            string hint = null,
            string answerExplanatoryNote = null,
            string feedbackCorrectAnswer = null,
            string feedbackWrongAnswer = null,
            int? level = null,
            bool? isSurveyTemplateQuestion = null,
            Guid? nextQuestionId = null)
        {
            return new Question
            {
                Type = QuestionType.ShortText,
                Title = title,
                CorrectAnswer = correctAnswer,
                Options = null,
                Hint = hint,
                AnswerExplanatoryNote = answerExplanatoryNote,
                FeedbackCorrectAnswer = feedbackCorrectAnswer,
                FeedbackWrongAnswer = feedbackWrongAnswer,
                Level = level,
                IsSurveyTemplateQuestion = isSurveyTemplateQuestion,
                NextQuestionId = nextQuestionId
            };
    }

        public static Question NewFreeResponse(
           string title,
           object correctAnswer = null,
           string hint = null,
           string answerExplanatoryNote = null,
           string feedbackCorrectAnswer = null,
           string feedbackWrongAnswer = null,
           int? level = null,
           bool? isSurveyTemplateQuestion = null,
           Guid? nextQuestionId = null)
        {
            return new Question
            {
                Type = QuestionType.FreeResponse,
                Title = title,
                CorrectAnswer = correctAnswer,
                Options = null,
                Hint = hint,
                AnswerExplanatoryNote = answerExplanatoryNote,
                FeedbackCorrectAnswer = feedbackCorrectAnswer,
                FeedbackWrongAnswer = feedbackWrongAnswer,
                Level = level,
                IsSurveyTemplateQuestion = isSurveyTemplateQuestion,
                NextQuestionId = nextQuestionId
            };
        }

        public static Question NewLongText(
            string title,
            string hint = null,
            string answerExplanatoryNote = null,
            string feedbackCorrectAnswer = null,
            string feedbackWrongAnswer = null,
            int? level = null,
            bool? isSurveyTemplateQuestion = null,
            Guid? nextQuestionId = null)
        {
            return new Question
            {
                Type = QuestionType.LongText,
                Title = title,
                CorrectAnswer = null,
                Options = null,
                Hint = hint,
                AnswerExplanatoryNote = answerExplanatoryNote,
                FeedbackCorrectAnswer = feedbackCorrectAnswer,
                FeedbackWrongAnswer = feedbackWrongAnswer,
                Level = level,
                IsSurveyTemplateQuestion = isSurveyTemplateQuestion,
                NextQuestionId = nextQuestionId
            };
    }

        public static Question NewTrueFalse(
            string title,
            IEnumerable<QuestionOption> options = null,
            bool? correctAnswer = null,
            string hint = null,
            string answerExplanatoryNote = null,
            string feedbackCorrectAnswer = null,
            string feedbackWrongAnswer = null,
            int? level = null,
            bool? isSurveyTemplateQuestion = null,
            Guid? nextQuestionId = null)
        {
            var defaultTrueOption = new QuestionOption(1, true, null, null, null, null, nextQuestionId);
            var defaultFalseOption = new QuestionOption(2, false, null, null, null, null, nextQuestionId);
            var defaultOptions = new List<QuestionOption>() { defaultTrueOption, defaultFalseOption };
            return new Question
            {
                Type = QuestionType.TrueFalse,
                Title = title,
                CorrectAnswer = correctAnswer,
                Options = options ?? defaultOptions,
                Hint = hint,
                AnswerExplanatoryNote = answerExplanatoryNote,
                FeedbackCorrectAnswer = feedbackCorrectAnswer,
                FeedbackWrongAnswer = feedbackWrongAnswer,
                Level = level,
                IsSurveyTemplateQuestion = isSurveyTemplateQuestion,
                NextQuestionId = nextQuestionId
            };
    }

        public static Question NewSingleChoice(
            string title,
            IEnumerable<QuestionOption> options,
            object correctAnswer = null,
            string hint = null,
            string answerExplanatoryNote = null,
            int? level = null,
            bool? isSurveyTemplateQuestion = null,
            Guid? nextQuestionId = null)
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
                FeedbackWrongAnswer = null,
                Level = level,
                IsSurveyTemplateQuestion = isSurveyTemplateQuestion,
                NextQuestionId = nextQuestionId
            };
    }

        public static Question NewMultipleChoice(
            string title,
            IEnumerable<QuestionOption> options,
            IEnumerable<object> correctAnswer = null,
            string hint = null,
            string answerExplanatoryNote = null,
            int? level = null,
            bool? isSurveyTemplateQuestion = null,
            Guid? nextQuestionId = null)
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
                FeedbackWrongAnswer = null,
                Level = level,
                IsSurveyTemplateQuestion = isSurveyTemplateQuestion,
                NextQuestionId = nextQuestionId
            };
    }

        public static Question NewFillInTheBlank(
            string title,
            IEnumerable<QuestionOption> options,
            IEnumerable<object> correctAnswer = null,
            string hint = null,
            string answerExplanatoryNote = null,
            string feedbackCorrectAnswer = null,
            string feedbackWrongAnswer = null,
            int? level = null,
            bool? isSurveyTemplateQuestion = null,
            Guid? nextQuestionId = null)
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
                FeedbackWrongAnswer = feedbackWrongAnswer,
                Level = level,
                IsSurveyTemplateQuestion = isSurveyTemplateQuestion,
                NextQuestionId = nextQuestionId
            };
    }

        public static Question NewDropDown(
            string title,
            IEnumerable<QuestionOption> options,
            object correctAnswer = null,
            string hint = null,
            string answerExplanatoryNote = null,
            string feedbackCorrectAnswer = null,
            string feedbackWrongAnswer = null,
            int? level = null,
            bool? isSurveyTemplateQuestion = null,
            Guid? nextQuestionId = null)
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
                FeedbackWrongAnswer = feedbackWrongAnswer,
                Level = level,
                IsSurveyTemplateQuestion = isSurveyTemplateQuestion,
                NextQuestionId = nextQuestionId
            };
    }

        public static Question DatePicker(
            string title,
            IEnumerable<QuestionOption> options,
            object correctAnswer = null,
            string hint = null,
            string answerExplanatoryNote = null,
            string feedbackCorrectAnswer = null,
            string feedbackWrongAnswer = null,
            int? level = null,
            bool? isSurveyTemplateQuestion = null,
            Guid? nextQuestionId = null)
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
                FeedbackWrongAnswer = feedbackWrongAnswer,
                Level = level,
                IsSurveyTemplateQuestion = isSurveyTemplateQuestion,
                NextQuestionId = nextQuestionId
            };
    }

        public static Question DateRangePicker(
            string title,
            IEnumerable<QuestionOption> options,
            object correctAnswer = null,
            string hint = null,
            string answerExplanatoryNote = null,
            string feedbackCorrectAnswer = null,
            string feedbackWrongAnswer = null,
            int? level = null,
            bool? isSurveyTemplateQuestion = null,
            Guid? nextQuestionId = null)
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
                FeedbackWrongAnswer = feedbackWrongAnswer,
                Level = level,
                IsSurveyTemplateQuestion = isSurveyTemplateQuestion,
                NextQuestionId = nextQuestionId
            };
        }

        public static Question NewScale(
            string title,
            IEnumerable<QuestionOption> options,
            string description = null,
            string hint = null,
            string answerExplanatoryNote = null,
            string feedbackCorrectAnswer = null,
            string feedbackWrongAnswer = null,
            int? level = null,
            bool? isSurveyTemplateQuestion = null,
            Guid? nextQuestionId = null)
        {
            return new Question
            {
                Type = QuestionType.Scale,
                Title = title,
                Options = options,
                Description = description,
                CorrectAnswer = null,
                Hint = hint,
                AnswerExplanatoryNote = answerExplanatoryNote,
                FeedbackCorrectAnswer = feedbackCorrectAnswer,
                FeedbackWrongAnswer = feedbackWrongAnswer,
                Level = level,
                IsSurveyTemplateQuestion = isSurveyTemplateQuestion,
                NextQuestionId = nextQuestionId
            };
        }

        public static Question NewCriteria(
            string title,
            IEnumerable<QuestionOption> options,
            string description = null,
            string hint = null,
            string answerExplanatoryNote = null,
            string feedbackCorrectAnswer = null,
            string feedbackWrongAnswer = null,
            int? level = null,
            bool? isSurveyTemplateQuestion = null,
            Guid? nextQuestionId = null)
        {
            return new Question
            {
                Type = QuestionType.Criteria,
                Title = title,
                Description = description,
                Options = options,
                CorrectAnswer = null,
                Hint = hint,
                AnswerExplanatoryNote = answerExplanatoryNote,
                FeedbackCorrectAnswer = feedbackCorrectAnswer,
                FeedbackWrongAnswer = feedbackWrongAnswer,
                Level = level,
                IsSurveyTemplateQuestion = isSurveyTemplateQuestion,
                NextQuestionId = nextQuestionId
            };
        }
    }
}
