using System;
using System.Collections.Generic;
using Microservice.LnaForm.Domain.ValueObjects.Form;

namespace Microservice.LnaForm.Application.Models
{
    public class FormQuestionOptionsOrderInfoModel
    {
        public FormQuestionOptionsOrderInfoModel()
        {
            // Do not remove this constructor. It is being used by JsonSerializer.Deserialize
        }

        public FormQuestionOptionsOrderInfoModel(FormQuestionOptionsOrderInfo formQuestionOptionsOrderInfo)
        {
            FormQuestionId = formQuestionOptionsOrderInfo.FormQuestionId;
            OptionCodeOrderList = formQuestionOptionsOrderInfo.OptionCodeOrderList;
        }

        public Guid FormQuestionId { get; set; }

        public IEnumerable<int> OptionCodeOrderList { get; set; } = new List<int>();
    }
}
