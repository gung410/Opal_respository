using System;
using Microservice.LnaForm.Domain.Entities;

namespace Microservice.LnaForm.Application.Models
{
    public class FormQuestionAnswerModel
    {
        public FormQuestionAnswerModel()
        {
            // Do not remove this constructor. It is being used by JsonSerializer.Deserialize
        }

        public FormQuestionAnswerModel(FormQuestionAnswer formQuestionAnswer)
        {
            FormAnswerId = formQuestionAnswer.FormAnswerId;
            FormQuestionId = formQuestionAnswer.FormQuestionId;
            AnswerValue = formQuestionAnswer.AnswerValue;
            SubmittedDate = formQuestionAnswer.SubmittedDate;
            SpentTimeInSeconds = formQuestionAnswer.SpentTimeInSeconds;
        }

        public Guid FormAnswerId { get; set; }

        public Guid FormQuestionId { get; set; }

        public object AnswerValue { get; set; }

        public DateTime? SubmittedDate { get; set; }

        public int? SpentTimeInSeconds { get; set; }
    }
}
