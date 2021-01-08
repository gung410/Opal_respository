using System;
using Microservice.StandaloneSurvey.Domain.Entities;

namespace Microservice.StandaloneSurvey.Application.Models
{
    public class SurveyQuestionAnswerModel
    {
        public SurveyQuestionAnswerModel()
        {
            // Do not remove this constructor. It is being used by JsonSerializer.Deserialize
        }

        public SurveyQuestionAnswerModel(SurveyQuestionAnswer formQuestionAnswer)
        {
            FormAnswerId = formQuestionAnswer.SurveyAnswerId;
            FormQuestionId = formQuestionAnswer.SurveyQuestionId;
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
