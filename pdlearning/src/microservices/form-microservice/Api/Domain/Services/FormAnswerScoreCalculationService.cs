using System;
using System.Collections.Generic;
using System.Linq;
using Microservice.Form.Common.Helpers;
using Microservice.Form.Domain.Entities;
using Microservice.Form.Domain.Enums;
using Microservice.Form.Domain.ValueObjects.Questions;
using FormEntity = Microservice.Form.Domain.Entities.Form;

namespace Microservice.Form.Domain.Services
{
    public class FormAnswerScoreCalculationService : IFormAnswerScoreCalculationService
    {
        public double CalcFormAnswerScore(FormEntity form, ICollection<FormQuestion> questions, FormAnswer formAnswer, IEnumerable<FormQuestionAnswer> formQuestionAnswers)
        {
            double score = 0;
            var formAnswerDic = formQuestionAnswers.ToDictionary(p => p.FormQuestionId, p => p);
            questions.ToList().ForEach(question =>
            {
                formAnswerDic.TryGetValue(question.Id, out var answer);
                score += answer != null ? CalculateQuestionAnswerScore(question, answer) : 0;
            });
            return score;
        }

        public double CalcFormAnswerScorePercentage(FormEntity form, ICollection<FormQuestion> questions, FormAnswer formAnswer, IEnumerable<FormQuestionAnswer> formQuestionAnswers)
        {
            var maxScore = CalcMaxScore(questions);
            return Math.Round((CalcFormAnswerScore(form, questions, formAnswer, formQuestionAnswers) / maxScore) * 100, 2);
        }

        public double CalcScorePercentage(double score, ICollection<FormQuestion> questions)
        {
            var maxScore = CalcMaxScore(questions);
            if (maxScore == 0)
            {
                return 0;
            }

            return Math.Round((score / maxScore) * 100, 2);
        }

        public double CalculateQuestionAnswerValueScore(FormQuestion formQuestion, object formQuestionAnswerValue)
        {
            if (formQuestion.Score == null || formQuestionAnswerValue == null)
            {
                return 0;
            }

            var formQuestionContent = formQuestion.BuildQuestion();
            switch (formQuestionContent.Type)
            {
                case QuestionType.ShortText:
                case QuestionType.SingleChoice:
                case QuestionType.TrueFalse:
                case QuestionType.FillInTheBlanks:
                case QuestionType.DropDown:
                    return formQuestionContent.IsAnswerCorrect(formQuestionAnswerValue)
                        ? formQuestion.Score.Value
                        : 0;

                case QuestionType.MultipleChoice:
                    return CalculateMultipleChoiceQuestionAnswerScore(
                        formQuestionContent,
                        formQuestion.Score.Value,
                        F.ParseObject<List<object>>(formQuestionContent.CorrectAnswer),
                        F.ParseObject<List<object>>(formQuestionAnswerValue));

                default:
                    return 0;
            }
        }

        public double CalculateMultipleChoiceQuestionAnswerScore(Question question, double maxScore, List<object> correctAnswer, List<object> answerValue)
        {
            if (correctAnswer == null || correctAnswer.Count() == 0)
            {
                return 0;
            }

            if (question.IsAnswerCorrect(answerValue))
            {
                return Math.Round(maxScore / correctAnswer.Count() * question.GetMultipleChoiceCorrectChoiceCount(answerValue), 2);
            }

            return 0;
        }

        public double CalcMaxScore(ICollection<FormQuestion> questions)
        {
            return questions.Aggregate(0D, (acc, formQuestion) => acc + (formQuestion.Score ?? 0));
        }

        public FormAnswerPassingStatus CalcFormAnswerPassingStatus(FormAnswer formAnswer, FormEntity form)
        {
            var updatePassingStatus = form.PassingMarkScore > formAnswer.Score
                || form.PassingMarkPercentage > formAnswer.ScorePercentage
                ? FormAnswerPassingStatus.Failed : FormAnswerPassingStatus.Passed;

            return updatePassingStatus;
        }

        public double CalculateManualQuestionAnswerScore(FormQuestion formQuestion, double score)
        {
            return score < 0 ?
                0 : score > formQuestion.Score ? formQuestion.Score.Value : score;
        }

        private double CalculateQuestionAnswerScore(FormQuestion formQuestion, FormQuestionAnswer formQuestionAnswer)
        {
            if (formQuestionAnswer.SubmittedDate == null)
            {
                return 0;
            }

            if (formQuestionAnswer.Score != null)
            {
                return formQuestionAnswer.Score.Value;
            }

            return CalculateQuestionAnswerValueScore(formQuestion, formQuestionAnswer.AnswerValue);
        }
    }
}
