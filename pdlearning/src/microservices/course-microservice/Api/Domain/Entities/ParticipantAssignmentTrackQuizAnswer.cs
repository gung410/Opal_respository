using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using Microservice.Course.Application.RequestDtos;
using Microservice.Course.Common.Extensions;
using Microservice.Course.Common.Helpers;
using Microservice.Course.Domain.Enums;
using Microservice.Course.Domain.ValueObjects;
using Thunder.Platform.Core.Domain.Auditing;
using Thunder.Platform.Core.Domain.Entities;
using Thunder.Platform.Core.Timing;

namespace Microservice.Course.Domain.Entities
{
    public class ParticipantAssignmentTrackQuizAnswer : FullAuditedEntity, ISoftDelete
    {
        public Guid QuizAssignmentFormId { get; set; }

        public float Score { get; set; }

        public float ScorePercentage { get; set; }

        public bool IsDeleted { get; set; }

        public virtual QuizAssignmentForm QuizAssignmentForm { get; set; }

        public virtual ICollection<ParticipantAssignmentTrackQuizQuestionAnswer> QuestionAnswers { get; set; } = new List<ParticipantAssignmentTrackQuizQuestionAnswer>();

        [JsonIgnore]
        public virtual ParticipantAssignmentTrack ParticipantAssignmentTrack { get; set; }

        public float CalcFormAnswerScore(
            IEnumerable<QuizAssignmentFormQuestion> questions,
            IEnumerable<ParticipantAssignmentTrackQuizQuestionAnswer> quizQuestionAnswers)
        {
            float score = 0;
            var formAnswerDic = quizQuestionAnswers.ToDictionary(
                p => p.QuizAssignmentFormQuestionId,
                p => p);
            questions.ToList().ForEach(question =>
            {
                formAnswerDic.TryGetValue(question.Id, out var answer);
                score += answer != null ? (CalculateQuestionAnswerScore(question, answer) ?? 0) : 0;
            });
            return score;
        }

        public float CalcScorePercentage(float score, IEnumerable<QuizAssignmentFormQuestion> questions)
        {
            var maxScore = CalcMaxScore(questions);
            if (maxScore == 0)
            {
                return 0;
            }

            return (float)Math.Round((score / maxScore) * 100, 2);
        }

        public float CalcMaxScore(IEnumerable<QuizAssignmentFormQuestion> questions)
        {
            return (float)questions.Aggregate(0D, (acc, formQuestion) => acc + formQuestion.MaxScore);
        }

        public float? CalculateQuestionAnswerValueScore(QuizAssignmentFormQuestion question, object formQuestionAnswerValue)
        {
            if (formQuestionAnswerValue == null)
            {
                return null;
            }

            var formQuestionContent = question.GetQuestionContent();
            switch (formQuestionContent.Type)
            {
                case QuestionType.FreeText:
                case QuestionType.SingleChoice:
                case QuestionType.TrueFalse:
                case QuestionType.FillInTheBlanks:
                case QuestionType.DropDown:
                case QuestionType.DatePicker:
                case QuestionType.DateRangePicker:
                    return formQuestionContent.IsAnswerCorrect(formQuestionAnswerValue)
                        ? question.MaxScore
                        : 0;

                case QuestionType.MultipleChoice:
                    return CalculateMultipleChoiceQuestionAnswerScore(
                        formQuestionContent,
                        question.MaxScore,
                        F.ParseObject<List<object>>(formQuestionContent.CorrectAnswer),
                        F.ParseObject<List<object>>(formQuestionAnswerValue));

                default:
                    return null;
            }
        }

        public float CalculateMultipleChoiceQuestionAnswerScore(Question question, float maxScore, List<object> correctAnswer, List<object> answerValue)
        {
            if (correctAnswer == null || !correctAnswer.Any())
            {
                return 0;
            }

            if (question.IsAnswerCorrect(answerValue))
            {
                return (float)Math.Round(maxScore / correctAnswer.Count() * question.GetMultipleChoiceCorrectChoiceCount(answerValue), 2);
            }

            return 0;
        }

        public void UpdateQuestionAnswerValues(List<ParticipantAssignmentTrackQuizQuestionAnswer> answers, List<QuizAssignmentFormQuestion> quizAssignmentFormQuestions, bool isSubmit)
        {
            var quizAssignmentFormQuestionsDic = quizAssignmentFormQuestions.ToDictionary(p => p.Id, p => p);

            QuestionAnswers.Update(
                answers,
                p => p.QuizAssignmentFormQuestionId,
                (source, target) =>
                {
                    source.AnswerValue = target.AnswerValue;

                    if (isSubmit)
                    {
                        source.SubmittedDate = Clock.Now;
                        source.Score = CalculateQuestionAnswerValueScore(quizAssignmentFormQuestionsDic[source.QuizAssignmentFormQuestionId], source.AnswerValue);
                    }
                },
                newAnswer =>
                {
                    newAnswer.QuizAnswerId = Id;
                    if (isSubmit)
                    {
                        newAnswer.SubmittedDate = Clock.Now;
                        newAnswer.Score = CalculateQuestionAnswerValueScore(quizAssignmentFormQuestionsDic[newAnswer.QuizAssignmentFormQuestionId], newAnswer.AnswerValue);
                    }

                    QuestionAnswers.Add(newAnswer);
                });

            if (isSubmit)
            {
               UpdateQuizAnswerTotalScore(quizAssignmentFormQuestions);
            }
        }

        public void MarkManualScore(
            IEnumerable<ParticipantAssignmentTrackQuizQuestionAnswer_ScoreInfo> questionAnswerScores,
            List<QuizAssignmentFormQuestion> quizAssignmentFormQuestions)
        {
            var questionAnswerScoresDic = questionAnswerScores.ToDictionary(x => x.QuizAssignmentFormQuestionId);
            foreach (var item in QuestionAnswers)
            {
                if (questionAnswerScoresDic.ContainsKey(item.QuizAssignmentFormQuestionId))
                {
                    var markScoreForQuizQuestionAnswer = questionAnswerScoresDic[item.QuizAssignmentFormQuestionId];
                    item.ManualScore = markScoreForQuizQuestionAnswer.Score;
                    item.ManualScoredBy = markScoreForQuizQuestionAnswer.ManualScoredBy;
                }
            }

            UpdateQuizAnswerTotalScore(quizAssignmentFormQuestions);
        }

        public void UpdateQuizAnswerTotalScore(List<QuizAssignmentFormQuestion> quizAssignmentFormQuestions)
        {
            Score = CalcFormAnswerScore(quizAssignmentFormQuestions, QuestionAnswers);
            ScorePercentage = CalcScorePercentage(Score, quizAssignmentFormQuestions);
        }

        private float? CalculateQuestionAnswerScore(QuizAssignmentFormQuestion question, ParticipantAssignmentTrackQuizQuestionAnswer quizQuestionAnswer)
        {
            if (quizQuestionAnswer.SubmittedDate == null)
            {
                return null;
            }

            if (quizQuestionAnswer.ManualScore != null)
            {
                return quizQuestionAnswer.ManualScore.Value;
            }

            if (quizQuestionAnswer.Score != null)
            {
                return quizQuestionAnswer.Score.Value;
            }

            return CalculateQuestionAnswerValueScore(question, quizQuestionAnswer.AnswerValue);
        }
    }
}
