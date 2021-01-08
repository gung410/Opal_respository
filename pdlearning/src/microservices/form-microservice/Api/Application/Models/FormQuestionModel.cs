using System;
using System.Collections.Generic;
using System.Linq;
using Microservice.Form.Domain.Entities;
using Microservice.Form.Domain.ValueObjects.Questions;

namespace Microservice.Form.Application.Models
{
    public class FormQuestionModel
    {
        public FormQuestionModel()
        {
            // Do not remove this constructor. It is being used by JsonSerializer.Deserialize
        }

        public FormQuestionModel(FormQuestion entity)
        {
            Id = entity.Id;
            FormId = entity.FormId;
            QuestionType = entity.Question_Type;
            QuestionTitle = entity.Question_Title;
            Description = entity.Description;
            QuestionCorrectAnswer = entity.Question_CorrectAnswer;
            QuestionOptions = entity.Question_Options?.Select(questionOption => (QuestionOptionModel)questionOption);
            QuestionHint = entity.Question_Hint;
            AnswerExplanatoryNote = entity.Question_AnswerExplanatoryNote;
            FeedbackCorrectAnswer = entity.Question_FeedbackCorrectAnswer;
            FeedbackWrongAnswer = entity.Question_FeedbackWrongAnswer;
            QuestionLevel = entity.Question_Level;
            RandomizedOptions = entity.RandomizedOptions;
            Score = entity.Score;
            Priority = entity.Priority;
            MinorPriority = entity.MinorPriority;
            ParentId = entity.ParentId;
            CreatedDate = entity.CreatedDate;
            ChangedDate = entity.ChangedDate;
            IsSurveyTemplateQuestion = entity.Question_IsSurveyTemplateQuestion;
            FormSectionId = entity.FormSectionId;
            NextQuestionId = entity.Question_NextQuestionId;
            IsScoreEnabled = entity.IsScoreEnabled;
        }

        public Guid Id { get; set; }

        public Guid FormId { get; set; }

        public QuestionType QuestionType { get; set; }

        public string QuestionTitle { get; set; }

        public string Description { get; set; }

        public object QuestionCorrectAnswer { get; set; }

        public IEnumerable<QuestionOptionModel> QuestionOptions { get; set; }

        public int Priority { get; set; }

        public int? MinorPriority { get; set; }

        public string QuestionHint { get; set; }

        public string AnswerExplanatoryNote { get; set; }

        public string FeedbackCorrectAnswer { get; set; }

        public string FeedbackWrongAnswer { get; set; }

        public int? QuestionLevel { get; set; }

        public bool? RandomizedOptions { get; set; }

        public bool? IsSurveyTemplateQuestion { get; set; }

        public Guid? ParentId { get; set; }

        public double? Score { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime? ChangedDate { get; set; }

        public Guid? NextQuestionId { get; set; }

        public Guid? FormSectionId { get; set; }

        public bool? IsScoreEnabled { get; set; }
    }
}
