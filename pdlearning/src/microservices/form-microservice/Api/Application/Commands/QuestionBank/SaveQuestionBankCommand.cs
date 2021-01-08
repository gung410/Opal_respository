using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microservice.Form.Application.Models;
using Microservice.Form.Domain.Entities;
using Microservice.Form.Domain.ValueObjects.Questions;
using Thunder.Platform.Cqrs;

namespace Microservice.Form.Application.Commands
{
    public class SaveQuestionBankCommand : BaseThunderCommand
    {
        public Guid Id { get; set; }

        public string Title { get; set; }

        public string QuestionGroupName { get; set; }

        public string QuestionTitle { get; set; }

        public QuestionType QuestionType { get; set; }

        public object QuestionCorrectAnswer { get; set; }

        public IEnumerable<QuestionOptionModel> QuestionOptions { get; set; } = new List<QuestionOptionModel>();

        public string QuestionHint { get; set; }

        public string QuestionAnswerExplanatoryNote { get; set; }

        public string QuestionFeedbackCorrectAnswer { get; set; }

        public string QuestionFeedbackWrongAnswer { get; set; }

        public int? QuestionLevel { get; set; }

        public bool? RandomizedOptions { get; set; }

        public double? Score { get; set; }

        public bool IsDeleted { get; set; }

        public bool IsScoreEnabled { get; set; }

        public bool IsCreation { get; set; }

        public Guid UserId { get; set; }

        public QuestionBank ToQuestionBank(Guid? questionGroupId = null)
        {
            return new QuestionBank
            {
                Id = Id,
                IsDeleted = IsDeleted,
                IsScoreEnabled = IsScoreEnabled,
                QuestionAnswerExplanatoryNote = QuestionAnswerExplanatoryNote,
                QuestionCorrectAnswer = QuestionCorrectAnswer,
                QuestionFeedbackCorrectAnswer = QuestionFeedbackCorrectAnswer,
                QuestionFeedbackWrongAnswer = QuestionFeedbackWrongAnswer,
                QuestionGroupId = questionGroupId,
                QuestionHint = QuestionHint,
                QuestionLevel = QuestionLevel,
                QuestionOptions = QuestionOptions?.Select(p => (QuestionOption)p),
                QuestionTitle = QuestionTitle,
                QuestionType = QuestionType,
                RandomizedOptions = RandomizedOptions,
                Score = Score,
                Title = Title,
                CreatedBy = UserId
            };
        }
    }
}
