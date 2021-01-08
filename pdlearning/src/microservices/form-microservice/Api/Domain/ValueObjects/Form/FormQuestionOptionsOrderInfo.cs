using System;
using System.Collections.Generic;

namespace Microservice.Form.Domain.ValueObjects.Form
{
    public class FormQuestionOptionsOrderInfo : BaseValueObject
    {
        public Guid FormQuestionId { get; set; }

        public IEnumerable<int> OptionCodeOrderList { get; set; } = new List<int>();
    }
}
