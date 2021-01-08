using System;
using System.Collections.Generic;
using System.Text.Json;
using Microservice.Course.Domain.Enums;
using Microservice.Course.Domain.ValueObjects;
using Thunder.Platform.Core.Domain.Auditing;
using Thunder.Platform.Core.Domain.Entities;
using Thunder.Platform.Core.Timing;
using Thunder.Platform.Core.Validation;

namespace Microservice.Course.Domain.Entities
{
    public class QuizAssignmentFormQuestion : FullAuditedEntity, ISoftDelete
    {
        public QuizAssignmentFormQuestion()
        {
        }

        public QuizAssignmentFormQuestion(Guid quizAssignmentFormId, int priority, float maxScore, QuestionType questionType, string questionTitle, object questionCorrectAnswer, IEnumerable<QuestionOption> questionOptions, string questionHint, string questionAnswerExplanatoryNote, string questionFeedbackCorrectAnswer, string questionFeedbackWrongAnswer, bool randomizedOptions)
        {
            QuizAssignmentFormId = quizAssignmentFormId;
            Priority = priority;
            MaxScore = maxScore;
            Question_Type = questionType;
            Question_Title = questionTitle;
            Question_CorrectAnswer = questionCorrectAnswer;
            Question_Options = questionOptions;
            Question_Hint = questionHint;
            Question_AnswerExplanatoryNote = questionAnswerExplanatoryNote;
            Question_FeedbackCorrectAnswer = questionFeedbackCorrectAnswer;
            Question_FeedbackWrongAnswer = questionFeedbackWrongAnswer;
            RandomizedOptions = randomizedOptions;
        }

        public Guid QuizAssignmentFormId { get; set; }

        public int Priority { get; set; }

        public float MaxScore { get; set; } = 1;

        public bool? RandomizedOptions { get; set; }

        public QuestionType Question_Type { get; set; }

        public string Question_Title { get; set; }

        public object Question_CorrectAnswer { get; set; }

        public IEnumerable<QuestionOption> Question_Options { get; set; }

        public string Question_Hint { get; set; }

        public string Question_AnswerExplanatoryNote { get; set; }

        public string Question_FeedbackCorrectAnswer { get; set; }

        public string Question_FeedbackWrongAnswer { get; set; }

        public bool IsDeleted { get; set; }

        public QuizAssignmentFormQuestion Clone(Guid newAssignmentId)
        {
            return new QuizAssignmentFormQuestion
            {
                Id = Guid.NewGuid(),
                CreatedDate = Clock.Now,
                QuizAssignmentFormId = newAssignmentId,
                Priority = Priority,
                MaxScore = MaxScore,
                RandomizedOptions = RandomizedOptions,
                Question_Type = Question_Type,
                Question_Title = Question_Title,
                Question_CorrectAnswer = Question_CorrectAnswer,
                Question_Options = Question_Options,
                Question_Hint = Question_Hint,
                Question_AnswerExplanatoryNote = Question_AnswerExplanatoryNote,
                Question_FeedbackCorrectAnswer = Question_FeedbackCorrectAnswer,
                Question_FeedbackWrongAnswer = Question_FeedbackWrongAnswer
            };
        }

        public Question GetQuestionContent()
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
                       Question_FeedbackWrongAnswer),
                QuestionType.TrueFalse => QuestionCreator.NewTrueFalse(
                        Question_Title,
                        Question_CorrectAnswer != null ? JsonSerializer.Deserialize<bool?>(Question_CorrectAnswer.ToString()?.ToLower()) : null,
                        Question_Hint,
                        Question_AnswerExplanatoryNote,
                        Question_FeedbackCorrectAnswer,
                        Question_FeedbackWrongAnswer),
                QuestionType.SingleChoice => QuestionCreator.NewSingleChoice(
                        Question_Title,
                        Question_Options,
                        Question_CorrectAnswer,
                        Question_Hint,
                        Question_AnswerExplanatoryNote),
                QuestionType.MultipleChoice => QuestionCreator.NewMultipleChoice(
                        Question_Title,
                        Question_Options,
                        Question_CorrectAnswer != null ? JsonSerializer.Deserialize<IEnumerable<object>>(Question_CorrectAnswer.ToString()) : null,
                        Question_Hint,
                        Question_AnswerExplanatoryNote),
                QuestionType.FreeText => QuestionCreator.NewFreeText(
                        Question_Title,
                        Question_CorrectAnswer,
                        Question_Hint,
                        Question_AnswerExplanatoryNote,
                        Question_FeedbackCorrectAnswer,
                        Question_FeedbackWrongAnswer),
                QuestionType.DropDown => QuestionCreator.NewDropDown(
                        Question_Title,
                        Question_Options,
                        Question_CorrectAnswer,
                        Question_Hint,
                        Question_AnswerExplanatoryNote,
                        Question_FeedbackCorrectAnswer,
                        Question_FeedbackWrongAnswer),
                QuestionType.DatePicker => QuestionCreator.DatePicker(
                        Question_Title,
                        Question_Options,
                        Question_CorrectAnswer,
                        Question_Hint,
                        Question_AnswerExplanatoryNote,
                        Question_FeedbackCorrectAnswer,
                        Question_FeedbackWrongAnswer),
                QuestionType.DateRangePicker => QuestionCreator.DateRangePicker(
                        Question_Title,
                        Question_Options,
                        Question_CorrectAnswer,
                        Question_Hint,
                        Question_AnswerExplanatoryNote,
                        Question_FeedbackCorrectAnswer,
                        Question_FeedbackWrongAnswer),
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
        }

        public Validation ValidateCanMarkScoreForQuestionAnswer(float score)
        {
            return Validation.ValidIf(
                score <= MaxScore && (Question_Type == QuestionType.FreeText || Question_Type == QuestionType.FillInTheBlanks),
                "Can only mark score for FreeText/FillInTheBlanks question answer and score must be less than or equal to MaxScore");
        }
    }
}
