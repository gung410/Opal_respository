using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text.Json;
using Microservice.Form.Common.Extensions;
using Microservice.Form.Common.Helpers;
using Thunder.Platform.Core.Exceptions;

namespace Microservice.Form.Domain.ValueObjects.Questions
{
    [SuppressMessage("Microsoft.Naming", "CA1724", Justification = "Toan Nguyen confirmed this.")]
    public class Question : BaseValueObject
    {
        public static readonly int MaxTitleLength = 3000;

        public Question()
        {
        }

        // TODO: It's horrible with following parameters and it was a downside of DDD. Good practices is less than 3 parameters.
        public Question(
            QuestionType type,
            string title,
            object correctAnswer,
            IEnumerable<QuestionOption> options,
            string hint,
            string answerExplanatoryNote,
            int? level,
            string description = null)
        {
            Type = type;
            Title = title;
            Description = description;
            CorrectAnswer = correctAnswer;
            Options = options;
            Hint = hint;
            AnswerExplanatoryNote = answerExplanatoryNote;
            Level = level;
        }

        public QuestionType Type { get; set; }

        public string Title { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public object CorrectAnswer { get; set; }

        public IEnumerable<QuestionOption> Options { get; set; }

        public string Hint { get; set; }

        public string AnswerExplanatoryNote { get; set; }

        public string FeedbackCorrectAnswer { get; set; }

        public string FeedbackWrongAnswer { get; set; }

        public int? Level { get; set; }

        public bool? IsSurveyTemplateQuestion { get; set; }

        public Guid? NextQuestionId { get; set; }

        public virtual bool IsAnswerCorrect(object answer)
        {
            if (answer == null || CorrectAnswer == null)
            {
                return false;
            }

            switch (Type)
            {
                case QuestionType.FillInTheBlanks:
                    return FillInTheBlankIsAnswerCorrect(answer);

                case QuestionType.MultipleChoice:
                    return MultipleChoiceIsAnswerCorrect(answer);

                default:
                    return DefaultCheckIsAnswerCorrect(answer);
            }
        }

        public int GetMultipleChoiceCorrectChoiceCount(List<object> answer)
        {
            if (Type != QuestionType.MultipleChoice || answer == null || CorrectAnswer == null)
            {
                return 0;
            }

            var parsedCorrectAnswer = F.ParseObject<List<object>>(CorrectAnswer).Select(_ => JsonSerializer.Serialize(_));
            if (!answer.Any() || !parsedCorrectAnswer.Any())
            {
                return 0;
            }

            return answer.Select(_ => JsonSerializer.Serialize(_)).Distinct().Count(p => parsedCorrectAnswer.Contains(p));
        }

        private bool MultipleChoiceIsAnswerCorrect(object answer)
        {
            var parsedAnswer = F.ParseObject<List<object>>(answer).Select(_ => JsonSerializer.Serialize(_));
            var parsedCorrectAnswer = F.ParseObject<List<object>>(CorrectAnswer).Select(_ => JsonSerializer.Serialize(_));
            if (parsedAnswer.Count() == 0 || parsedCorrectAnswer.Count() == 0)
            {
                return false;
            }

            return parsedCorrectAnswer.ContainsAll(parsedAnswer.ToArray());
        }

        private bool FillInTheBlankIsAnswerCorrect(object answer)
        {
            var parsedAnswer = F.ParseObject<List<string>>(answer).ToList().ConvertAll(x => x.ToLower());
            var parsedCorrectAnswer = F.ParseObject<List<string>>(CorrectAnswer).ConvertAll(x => x.ToLower());
            if (!parsedAnswer.Any() || !parsedCorrectAnswer.Any())
            {
                return false;
            }

            if (parsedCorrectAnswer.Any(p => !p.Any()))
            {
                throw new BusinessLogicException("Correct answers for a blank in FillInTheBlanksQuestion cannot be empty");
            }

            for (int i = 0; i < parsedCorrectAnswer.Count(); i++)
            {
                if (!parsedCorrectAnswer[i].Contains(parsedAnswer[i]))
                {
                    return false;
                }
            }

            return true;
        }

        private bool DefaultCheckIsAnswerCorrect(object answer)
        {
            return JsonSerializer.Serialize(answer).ToLower() == JsonSerializer.Serialize(CorrectAnswer).ToLower();
        }
    }
}
