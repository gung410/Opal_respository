using System;
using System.Collections.Generic;
using System.Linq;
using Microservice.Course.Domain.Entities;
using Microservice.Course.Domain.Enums;

namespace Microservice.Course.Application.Models
{
    public class QuizAssignmentFormQuestionModel
    {
        public QuizAssignmentFormQuestionModel()
        {
        }

        public QuizAssignmentFormQuestionModel(QuizAssignmentFormQuestion quizAssignmentFormQuestion, bool forLearnerAnswer)
        {
            Id = quizAssignmentFormQuestion.Id;
            QuizAssignmentFormId = quizAssignmentFormQuestion.QuizAssignmentFormId;
            Priority = quizAssignmentFormQuestion.Priority;
            MaxScore = quizAssignmentFormQuestion.MaxScore;
            RandomizedOptions = quizAssignmentFormQuestion.RandomizedOptions;
            Question_Type = quizAssignmentFormQuestion.Question_Type;
            Question_Title = quizAssignmentFormQuestion.Question_Title;
            Question_CorrectAnswer = forLearnerAnswer == false ? quizAssignmentFormQuestion.Question_CorrectAnswer : null;
            Question_Options = quizAssignmentFormQuestion.Question_Options?.Select(x => new QuestionOptionModel(x));
            Question_Hint = quizAssignmentFormQuestion.Question_Hint;
            Question_AnswerExplanatoryNote = quizAssignmentFormQuestion.Question_AnswerExplanatoryNote;
            Question_FeedbackCorrectAnswer = quizAssignmentFormQuestion.Question_FeedbackCorrectAnswer;
            Question_FeedbackWrongAnswer = quizAssignmentFormQuestion.Question_FeedbackWrongAnswer;
        }

        public Guid? Id { get; set; }

        public Guid QuizAssignmentFormId { get; set; }

        public int Priority { get; set; }

        public float MaxScore { get; set; } = 1;

        public QuestionType Question_Type { get; set; }

        public bool? RandomizedOptions { get; set; }

        public string Question_Title { get; set; }

        public object Question_CorrectAnswer { get; set; }

        public IEnumerable<QuestionOptionModel> Question_Options { get; set; }

        public string Question_Hint { get; set; }

        public string Question_AnswerExplanatoryNote { get; set; }

        public string Question_FeedbackCorrectAnswer { get; set; }

        public string Question_FeedbackWrongAnswer { get; set; }

        public QuizAssignmentFormQuestion ToEntity(Guid? newQuizAssignmentFormId = null)
        {
            return new QuizAssignmentFormQuestion
            {
                Id = Id ?? Guid.NewGuid(),
                QuizAssignmentFormId = newQuizAssignmentFormId ?? QuizAssignmentFormId,
                Priority = Priority,
                MaxScore = MaxScore,
                Question_Type = Question_Type,
                Question_Title = Question_Title,
                Question_CorrectAnswer = Question_CorrectAnswer,
                Question_Options = Question_Options?.Select(x => x.ToEntity()),
                Question_Hint = Question_Hint,
                Question_AnswerExplanatoryNote = Question_AnswerExplanatoryNote,
                Question_FeedbackCorrectAnswer = Question_FeedbackCorrectAnswer,
                Question_FeedbackWrongAnswer = Question_FeedbackWrongAnswer,
            };
        }
    }
}
