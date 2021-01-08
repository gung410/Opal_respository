using System;
using System.Collections.Generic;

namespace Microservice.Form.Domain.ValueObjects.Form
{
    public class FormAnswerFormMetaData : BaseValueObject
    {
        public IEnumerable<Guid> QuestionIdOrderList { get; set; }

        public IEnumerable<FormQuestionOptionsOrderInfo> FormQuestionOptionsOrderInfoList { get; set; }
    }
}
