using System;
using System.Collections.Generic;
using Microservice.StandaloneSurvey.Domain.ValueObjects.Form;

namespace Microservice.StandaloneSurvey.Application.Models
{
    public class SurveyQuestionOptionsOrderInfoModel
    {
        public SurveyQuestionOptionsOrderInfoModel()
        {
            // Do not remove this constructor. It is being used by JsonSerializer.Deserialize
        }

        public SurveyQuestionOptionsOrderInfoModel(SurveyQuestionOptionsOrderInfo surveyQuestionOptionsOrderInfo)
        {
            FormQuestionId = surveyQuestionOptionsOrderInfo.SurveyQuestionId;
            OptionCodeOrderList = surveyQuestionOptionsOrderInfo.OptionCodeOrderList;
        }

        public Guid FormQuestionId { get; set; }

        public IEnumerable<int> OptionCodeOrderList { get; set; } = new List<int>();
    }
}
