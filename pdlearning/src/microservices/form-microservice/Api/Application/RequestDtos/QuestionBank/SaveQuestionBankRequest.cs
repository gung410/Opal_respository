using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microservice.Form.Application.Commands;
using Microservice.Form.Application.Models;
using Microservice.Form.Domain.ValueObjects.Questions;

namespace Microservice.Form.Application.RequestDtos
{
    public class SaveQuestionBankRequest
    {
        public Guid? Id { get; set; }

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

        public SaveQuestionBankCommand ToCommand(Guid userId, bool isCreation = false)
        {
            return new SaveQuestionBankCommand
            {
                Id = Id.HasValue ? Id.Value : Guid.NewGuid(),
                IsDeleted = IsDeleted,
                IsScoreEnabled = IsScoreEnabled,
                QuestionAnswerExplanatoryNote = QuestionAnswerExplanatoryNote,
                QuestionCorrectAnswer = QuestionCorrectAnswer,
                QuestionFeedbackCorrectAnswer = QuestionFeedbackCorrectAnswer,
                QuestionFeedbackWrongAnswer = QuestionFeedbackWrongAnswer,
                QuestionGroupName = QuestionGroupName,
                QuestionHint = QuestionHint,
                QuestionLevel = QuestionLevel,
                QuestionOptions = QuestionOptions,
                QuestionTitle = QuestionTitle,
                QuestionType = QuestionType,
                RandomizedOptions = RandomizedOptions,
                Score = Score,
                Title = Title,
                IsCreation = isCreation,
                UserId = userId
            };
        }
    }
}
