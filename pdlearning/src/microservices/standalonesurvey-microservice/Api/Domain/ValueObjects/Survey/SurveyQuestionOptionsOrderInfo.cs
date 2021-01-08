using System;
using System.Collections.Generic;

namespace Microservice.StandaloneSurvey.Domain.ValueObjects.Form
{
    public class SurveyQuestionOptionsOrderInfo : BaseValueObject
    {
        public Guid SurveyQuestionId { get; set; }

        public IEnumerable<int> OptionCodeOrderList { get; set; } = new List<int>();
    }
}
