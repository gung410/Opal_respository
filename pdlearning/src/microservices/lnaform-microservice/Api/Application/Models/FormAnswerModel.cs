using System;
using System.Collections.Generic;
using System.Linq;
using Microservice.LnaForm.Domain.Entities;

namespace Microservice.LnaForm.Application.Models
{
    public class FormAnswerModel
    {
        public FormAnswerModel()
        {
            // Do not remove this constructor. It is being used by JsonSerializer.Deserialize
        }

        public FormAnswerModel(FormAnswer formAnswer, IEnumerable<FormQuestionAnswer> formQuestionAnswers)
        {
            Id = formAnswer.Id;
            FormId = formAnswer.FormId;
            ResourceId = formAnswer.ResourceId;
            StartDate = formAnswer.StartDate;
            EndDate = formAnswer.EndDate;
            SubmitDate = formAnswer.SubmitDate;
            Attempt = formAnswer.Attempt;
            FormMetaData = new FormAnswerFormMetaDataModel(formAnswer.FormMetaData);
            OwnerId = formAnswer.OwnerId;
            QuestionAnswers = formQuestionAnswers.Select(p => new FormQuestionAnswerModel(p)).ToList();
            IsCompleted = formAnswer.IsCompleted;
            CreatedDate = formAnswer.CreatedDate;
            ChangedDate = formAnswer.ChangedDate;
        }

        public Guid Id { get; set; }

        public Guid FormId { get; set; }

        public Guid? ResourceId { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public DateTime? SubmitDate { get; set; }

        public short Attempt { get; set; } = 1;

        public FormAnswerFormMetaDataModel FormMetaData { get; set; }

        public Guid OwnerId { get; set; }

        public ICollection<FormQuestionAnswerModel> QuestionAnswers { get; set; } = new List<FormQuestionAnswerModel>();

        public bool IsCompleted { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime? ChangedDate { get; set; }
    }
}
