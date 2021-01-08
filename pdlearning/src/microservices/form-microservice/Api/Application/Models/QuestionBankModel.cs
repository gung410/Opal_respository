using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microservice.Form.Domain;
using Microservice.Form.Domain.Entities;
using Microservice.Form.Domain.ValueObjects.Questions;
using Thunder.Platform.Core.Domain.Entities;

namespace Microservice.Form.Application.Models
{
    public class QuestionBankModel
    {
        public QuestionBankModel()
        {
            // Do not remove this constructor. It is being used by JsonSerializer.Deserialize
        }

        public QuestionBankModel(QuestionBank entity)
        {
            Id = entity.Id;
            IsDeleted = entity.IsDeleted;
            IsScoreEnabled = entity.IsScoreEnabled;
            QuestionAnswerExplanatoryNote = entity.QuestionAnswerExplanatoryNote;
            QuestionCorrectAnswer = entity.QuestionCorrectAnswer;
            QuestionFeedbackCorrectAnswer = entity.QuestionFeedbackCorrectAnswer;
            QuestionFeedbackWrongAnswer = entity.QuestionFeedbackWrongAnswer;
            QuestionGroupId = entity.QuestionGroupId;
            QuestionHint = entity.QuestionHint;
            QuestionLevel = entity.QuestionLevel;
            QuestionOptions = entity.QuestionOptions?.Select(p => (QuestionOptionModel)p);
            QuestionTitle = entity.QuestionTitle;
            QuestionType = entity.QuestionType;
            RandomizedOptions = entity.RandomizedOptions;
            Score = entity.Score;
            Title = entity.Title;
            CreatedDate = entity.CreatedDate;
            ChangedDate = entity.ChangedDate;
        }

        public Guid? Id { get; set; }

        public string Title { get; set; }

        public Guid? QuestionGroupId { get; set; }

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

        public DateTime CreatedDate { get; set; }

        public DateTime? ChangedDate { get; set; }
    }
}
