using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Microservice.Course.Common.Extensions;
using Microservice.Course.Common.Helpers;
using Microservice.Course.Domain.Enums;
using Thunder.Platform.Core.Exceptions;
using Thunder.Platform.Core.Extensions;

namespace Microservice.Course.Domain.ValueObjects
{
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
            string answerExplanatoryNote)
        {
            Type = type;
            Title = title;
            CorrectAnswer = correctAnswer;
            Options = options;
            Hint = hint;
            AnswerExplanatoryNote = answerExplanatoryNote;
        }

        public QuestionType Type { get; set; }

        public string Title { get; set; } = string.Empty;

        public object CorrectAnswer { get; set; }

        public IEnumerable<QuestionOption> Options { get; set; }

        public string Hint { get; set; }

        public string AnswerExplanatoryNote { get; set; }

        public string FeedbackCorrectAnswer { get; set; }

        public string FeedbackWrongAnswer { get; set; }

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

                case QuestionType.DatePicker:
                    return SingleDateIsAnswerCorrect(answer);

                case QuestionType.DateRangePicker:
                    return RangeDateIsAnswerCorrect(answer);

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

            var parsedCorrectAnswer = F.ParseObject<List<object>>(CorrectAnswer).SelectList(_ => JsonSerializer.Serialize(_));
            if (!answer.Any() || !parsedCorrectAnswer.Any())
            {
                return 0;
            }

            return answer.Select(_ => JsonSerializer.Serialize(_)).Distinct().Count(p => parsedCorrectAnswer.Contains(p));
        }

        private bool MultipleChoiceIsAnswerCorrect(object answer)
        {
            var parsedAnswer = F.ParseObject<List<object>>(answer)
                .SelectList(_ => JsonSerializer.Serialize(_));
            var parsedCorrectAnswer = F.ParseObject<List<object>>(CorrectAnswer)
                .SelectList(_ => JsonSerializer.Serialize(_));
            if (!parsedAnswer.Any() || !parsedCorrectAnswer.Any())
            {
                return false;
            }

            return parsedCorrectAnswer.ContainsAll(parsedAnswer.ToArray()) && parsedAnswer.Count == parsedCorrectAnswer.Count;
        }

        private bool FillInTheBlankIsAnswerCorrect(object answer)
        {
            var parsedAnswer = F.ParseObject<List<string>>(answer);
            var parsedCorrectAnswer = F.ParseObject<List<string>>(CorrectAnswer);
            if (!parsedAnswer.Any() || !parsedCorrectAnswer.Any())
            {
                return false;
            }

            if (parsedCorrectAnswer.Any(p => !p.Any()))
            {
                throw new GeneralException("Correct answers for a blank in FillInTheBlanksQuestion cannot be empty");
            }

            for (int i = 0; i < parsedCorrectAnswer.Count; i++)
            {
                if (!parsedCorrectAnswer[i].Contains(parsedAnswer[i]))
                {
                    return false;
                }
            }

            return true;
        }

        private bool SingleDateIsAnswerCorrect(object answer)
        {
            if (answer == null || CorrectAnswer == null)
            {
                throw new GeneralException("Answer and correct answer can not be null");
            }

            return DateTime.Compare(DateTime.Parse(answer.ToString()!).Date, DateTime.Parse(CorrectAnswer.ToString()!).Date) == 0;
        }

        private bool RangeDateIsAnswerCorrect(object answer)
        {
            var parsedAnswer = F.ParseObject<List<DateTime>>(answer);
            var parsedCorrectAnswer = F.ParseObject<List<DateTime>>(CorrectAnswer);
            if (!parsedAnswer.Any() || !parsedCorrectAnswer.Any())
            {
                return false;
            }

            if (!parsedAnswer.Any() || !parsedCorrectAnswer.Any())
            {
                return false;
            }

            if (parsedAnswer.Count != parsedCorrectAnswer.Count)
            {
                return false;
            }

            for (int i = 0; i < parsedCorrectAnswer.Count; i++)
            {
                if (DateTime.Compare(parsedCorrectAnswer[i].Date, parsedAnswer[i].Date) != 0)
                {
                    return false;
                }
            }

            return true;
        }

        private bool DefaultCheckIsAnswerCorrect(object answer)
        {
            return JsonSerializer.Serialize(answer) == JsonSerializer.Serialize(CorrectAnswer);
        }
    }
}
