using System;
using System.Collections.Generic;
using System.Linq;
using Microservice.LnaForm.Domain.Entities;
using Microservice.LnaForm.Domain.ValueObjects.Questions;

namespace Microservice.LnaForm.Application.Models
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
            QuestionType = entity.QuestionType;
            QuestionTitle = entity.Title;
            QuestionCorrectAnswer = entity.CorrectAnswer;
            QuestionOptions = entity.Options?.Select(questionOption => (QuestionOptionModel)questionOption);
            Priority = entity.Priority;
            MinorPriority = entity.MinorPriority;
            ParentId = entity.ParentId;
            CreatedDate = entity.CreatedDate;
            ChangedDate = entity.ChangedDate;
            FormSectionId = entity.FormSectionId;
            NextQuestionId = entity.NextQuestionId;
        }

        public Guid Id { get; set; }

        public Guid FormId { get; set; }

        public QuestionType QuestionType { get; set; }

        public string QuestionTitle { get; set; }

        public object QuestionCorrectAnswer { get; set; }

        public IEnumerable<QuestionOptionModel> QuestionOptions { get; set; }

        public int Priority { get; set; }

        public int? MinorPriority { get; set; }

        public Guid? ParentId { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime? ChangedDate { get; set; }

        public Guid? NextQuestionId { get; set; }

        public Guid? FormSectionId { get; set; }
    }
}
