using System;
using System.Collections.Generic;
using System.Linq;
using Microservice.StandaloneSurvey.Domain.Entities;

namespace Microservice.StandaloneSurvey.Application.Models
{
    public class SurveyAnswerModel
    {
        public SurveyAnswerModel()
        {
            // Do not remove this constructor. It is being used by JsonSerializer.Deserialize
        }

        public SurveyAnswerModel(SurveyAnswer surveyAnswer, IEnumerable<SurveyQuestionAnswer> formQuestionAnswers)
        {
            Id = surveyAnswer.Id;
            FormId = surveyAnswer.FormId;
            ResourceId = surveyAnswer.ResourceId;
            StartDate = surveyAnswer.StartDate;
            EndDate = surveyAnswer.EndDate;
            SubmitDate = surveyAnswer.SubmitDate;
            Attempt = surveyAnswer.Attempt;
            FormMetaData = new SurveyAnswerSurveyMetaDataModel(surveyAnswer.SurveyAnswerFormMetaData);
            OwnerId = surveyAnswer.OwnerId;
            QuestionAnswers = formQuestionAnswers.Select(p => new SurveyQuestionAnswerModel(p)).ToList();
            IsCompleted = surveyAnswer.IsCompleted;
            CreatedDate = surveyAnswer.CreatedDate;
            ChangedDate = surveyAnswer.ChangedDate;
        }

        public Guid Id { get; set; }

        public Guid FormId { get; set; }

        public Guid? ResourceId { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public DateTime? SubmitDate { get; set; }

        public short Attempt { get; set; } = 1;

        public SurveyAnswerSurveyMetaDataModel FormMetaData { get; set; }

        public Guid OwnerId { get; set; }

        public ICollection<SurveyQuestionAnswerModel> QuestionAnswers { get; set; } = new List<SurveyQuestionAnswerModel>();

        public bool IsCompleted { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime? ChangedDate { get; set; }
    }
}
