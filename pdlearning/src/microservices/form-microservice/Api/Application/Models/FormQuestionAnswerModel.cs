using System;
using System.Collections.Generic;
using Microservice.Form.Domain.Entities;

namespace Microservice.Form.Application.Models
{
    public class FormQuestionAnswerModel
    {
        public FormQuestionAnswerModel()
        {
            // Do not remove this constructor. It is being used by JsonSerializer.Deserialize
        }

        public FormQuestionAnswerModel(FormQuestionAnswer formQuestionAnswer, List<FormAnswerAttachmentModel> formAttachments = null)
        {
            Id = formQuestionAnswer.Id;
            FormAnswerId = formQuestionAnswer.FormAnswerId;
            FormQuestionId = formQuestionAnswer.FormQuestionId;
            AnswerValue = formQuestionAnswer.AnswerValue;
            FormAnswerAttachments = formAttachments;
            Score = formQuestionAnswer.Score;
            MaxScore = formQuestionAnswer.MaxScore;
            ScoredBy = formQuestionAnswer.ScoredBy;
            AnswerFeedback = formQuestionAnswer.AnswerFeedback;
            SubmittedDate = formQuestionAnswer.SubmittedDate;
            SpentTimeInSeconds = formQuestionAnswer.SpentTimeInSeconds;
        }

        public Guid Id { get; set; }

        public Guid FormAnswerId { get; set; }

        public Guid FormQuestionId { get; set; }

        public object AnswerValue { get; set; }

        public IEnumerable<FormAnswerAttachmentModel> FormAnswerAttachments { get; set; }

        public double? MaxScore { get; set; }

        public double? Score { get; set; }

        public Guid? ScoredBy { get; set; }

        public string AnswerFeedback { get; set; }

        public DateTime? SubmittedDate { get; set; }

        public int? SpentTimeInSeconds { get; set; }
    }
}
