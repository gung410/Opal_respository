using System;
using System.Collections.Generic;
using System.Text.Json;
using Microservice.Form.Domain.ValueObjects.Questions;

namespace Microservice.Form.Domain
{
    public abstract class OwnQuestionEntity : BaseEntity, IOwnQuestionEntity
    {
        public QuestionType Question_Type { get; set; }

        public string Question_Title { get; set; }

        public object Question_CorrectAnswer { get; set; }

        public IEnumerable<QuestionOption> Question_Options { get; set; }

        public string Question_Hint { get; set; }

        public string Question_AnswerExplanatoryNote { get; set; }

        public string Question_FeedbackCorrectAnswer { get; set; }

        public string Question_FeedbackWrongAnswer { get; set; }

        public int? Question_Level { get; set; }

        public bool? Question_IsSurveyTemplateQuestion { get; set; }

        public Guid? Question_NextQuestionId { get; set; }

        public Question BuildQuestion()
        {
            return Question_Type switch
            {
                QuestionType.FillInTheBlanks => QuestionCreator.NewFillInTheBlank(
                       Question_Title,
                       Question_Options,
                       Question_CorrectAnswer != null ? JsonSerializer.Deserialize<IEnumerable<object>>(Question_CorrectAnswer.ToString()) : null,
                       Question_Hint,
                       Question_AnswerExplanatoryNote,
                       Question_FeedbackCorrectAnswer,
                       Question_FeedbackWrongAnswer,
                       Question_Level,
                       Question_IsSurveyTemplateQuestion,
                       Question_NextQuestionId),
                QuestionType.TrueFalse => QuestionCreator.NewTrueFalse(
                        Question_Title,
                        Question_Options,
                        Question_CorrectAnswer != null ? JsonSerializer.Deserialize<bool?>(Question_CorrectAnswer.ToString().ToLower()) : null,
                        Question_Hint,
                        Question_AnswerExplanatoryNote,
                        Question_FeedbackCorrectAnswer,
                        Question_FeedbackWrongAnswer,
                        Question_Level,
                        Question_IsSurveyTemplateQuestion,
                        Question_NextQuestionId),
                QuestionType.SingleChoice => QuestionCreator.NewSingleChoice(
                        Question_Title,
                        Question_Options,
                        Question_CorrectAnswer,
                        Question_Hint,
                        Question_AnswerExplanatoryNote,
                        Question_Level,
                        Question_IsSurveyTemplateQuestion,
                        Question_NextQuestionId),
                QuestionType.MultipleChoice => QuestionCreator.NewMultipleChoice(
                        Question_Title,
                        Question_Options,
                        Question_CorrectAnswer != null ? JsonSerializer.Deserialize<IEnumerable<object>>(Question_CorrectAnswer.ToString()) : null,
                        Question_Hint,
                        Question_AnswerExplanatoryNote,
                        Question_Level,
                        Question_IsSurveyTemplateQuestion,
                        Question_NextQuestionId),
                QuestionType.ShortText => QuestionCreator.NewShortText(
                        Question_Title,
                        Question_CorrectAnswer,
                        Question_Hint,
                        Question_AnswerExplanatoryNote,
                        Question_FeedbackCorrectAnswer,
                        Question_FeedbackWrongAnswer,
                        Question_Level,
                        Question_IsSurveyTemplateQuestion,
                        Question_NextQuestionId),
                QuestionType.FreeResponse => QuestionCreator.NewFreeResponse(
                        Question_Title,
                        Question_CorrectAnswer,
                        Question_Hint,
                        Question_AnswerExplanatoryNote,
                        Question_FeedbackCorrectAnswer,
                        Question_FeedbackWrongAnswer,
                        Question_Level,
                        Question_IsSurveyTemplateQuestion,
                        Question_NextQuestionId),
                QuestionType.LongText => QuestionCreator.NewLongText(
                        Question_Title,
                        Question_Hint,
                        Question_AnswerExplanatoryNote,
                        Question_FeedbackCorrectAnswer,
                        Question_FeedbackWrongAnswer,
                        Question_Level,
                        Question_IsSurveyTemplateQuestion,
                        Question_NextQuestionId),
                QuestionType.DropDown => QuestionCreator.NewDropDown(
                        Question_Title,
                        Question_Options,
                        Question_CorrectAnswer,
                        Question_Hint,
                        Question_AnswerExplanatoryNote,
                        Question_FeedbackCorrectAnswer,
                        Question_FeedbackWrongAnswer,
                        Question_Level,
                        Question_IsSurveyTemplateQuestion,
                        Question_NextQuestionId),
                QuestionType.DatePicker => QuestionCreator.DatePicker(
                        Question_Title,
                        Question_Options,
                        Question_CorrectAnswer,
                        Question_Hint,
                        Question_AnswerExplanatoryNote,
                        Question_FeedbackCorrectAnswer,
                        Question_FeedbackWrongAnswer,
                        Question_Level,
                        Question_IsSurveyTemplateQuestion,
                        Question_NextQuestionId),
                QuestionType.DateRangePicker => QuestionCreator.DateRangePicker(
                        Question_Title,
                        Question_Options,
                        Question_CorrectAnswer,
                        Question_Hint,
                        Question_AnswerExplanatoryNote,
                        Question_FeedbackCorrectAnswer,
                        Question_FeedbackWrongAnswer,
                        Question_Level,
                        Question_IsSurveyTemplateQuestion,
                        Question_NextQuestionId),
                _ => null,
            };
        }

        public void SetQuestionContent(Question question)
        {
            Question_Type = question.Type;
            Question_Title = question.Title;
            Question_CorrectAnswer = question.CorrectAnswer;
            Question_Options = question.Options;
            Question_Hint = question.Hint;
            Question_AnswerExplanatoryNote = question.AnswerExplanatoryNote;
            Question_FeedbackCorrectAnswer = question.FeedbackCorrectAnswer;
            Question_FeedbackWrongAnswer = question.FeedbackWrongAnswer;
            Question_Level = question.Level;
            Question_IsSurveyTemplateQuestion = question.IsSurveyTemplateQuestion;
        }
    }
}
