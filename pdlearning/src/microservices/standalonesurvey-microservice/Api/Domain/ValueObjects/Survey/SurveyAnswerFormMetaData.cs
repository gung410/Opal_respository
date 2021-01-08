using System;
using System.Collections.Generic;
using Microservice.StandaloneSurvey.Domain.ValueObjects.Form;

namespace Microservice.StandaloneSurvey.Domain.ValueObjects.Survey
{
    public class SurveyAnswerFormMetaData : BaseValueObject
    {
        public IEnumerable<Guid> QuestionIdOrderList { get; set; } // TODO: (NhonHT) should be removed? -> using with Form.RandomizedQuestions has been removed

        public IEnumerable<SurveyQuestionOptionsOrderInfo> FormQuestionOptionsOrderInfoList { get; set; }
    }
}
