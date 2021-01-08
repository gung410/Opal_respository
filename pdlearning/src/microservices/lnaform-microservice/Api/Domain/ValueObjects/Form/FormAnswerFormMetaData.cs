using System;
using System.Collections.Generic;

namespace Microservice.LnaForm.Domain.ValueObjects.Form
{
    public class FormAnswerFormMetaData : BaseValueObject
    {
        public IEnumerable<Guid> QuestionIdOrderList { get; set; } // TODO: (NhonHT) should be removed? -> using with Form.RandomizedQuestions has been removed

        public IEnumerable<FormQuestionOptionsOrderInfo> FormQuestionOptionsOrderInfoList { get; set; }
    }
}
