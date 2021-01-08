using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Microservice.StandaloneSurvey.Domain.ValueObjects.Questions
{
    [SuppressMessage("Microsoft.Naming", "CA1724", Justification = "Toan Nguyen confirmed this.")]
    public class Question : BaseValueObject
    {
        public static readonly int MaxTitleLength = 3000;

        public Question()
        {
        }

        public Question(
            QuestionType type,
            string title,
            object correctAnswer,
            IEnumerable<QuestionOption> options)
        {
            Type = type;
            Title = title;
            CorrectAnswer = correctAnswer;
            Options = options;
        }

        public QuestionType Type { get; set; }

        public string Title { get; set; } = string.Empty;

        public object CorrectAnswer { get; set; }

        public IEnumerable<QuestionOption> Options { get; set; }

        public Guid? NextQuestionId { get; set; }
    }
}
