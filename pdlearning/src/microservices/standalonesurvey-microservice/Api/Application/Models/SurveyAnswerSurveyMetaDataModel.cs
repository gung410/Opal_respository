using System;
using System.Collections.Generic;
using System.Linq;
using Microservice.StandaloneSurvey.Domain.ValueObjects.Survey;

namespace Microservice.StandaloneSurvey.Application.Models
{
    public class SurveyAnswerSurveyMetaDataModel
    {
        public SurveyAnswerSurveyMetaDataModel()
        {
            // Do not remove this constructor. It is being used by JsonSerializer.Deserialize
        }

        public SurveyAnswerSurveyMetaDataModel(SurveyAnswerFormMetaData surveyAnswerFormMeta)
        {
            QuestionIdOrderList = surveyAnswerFormMeta.QuestionIdOrderList;
            FormQuestionOptionsOrderInfoList = surveyAnswerFormMeta.FormQuestionOptionsOrderInfoList?.Select(p => new SurveyQuestionOptionsOrderInfoModel(p));
        }

        public IEnumerable<Guid> QuestionIdOrderList { get; set; }

        public IEnumerable<SurveyQuestionOptionsOrderInfoModel> FormQuestionOptionsOrderInfoList { get; set; }
    }
}
