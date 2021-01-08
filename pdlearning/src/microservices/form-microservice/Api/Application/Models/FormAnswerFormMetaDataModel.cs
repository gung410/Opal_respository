using System;
using System.Collections.Generic;
using System.Linq;
using Microservice.Form.Domain.ValueObjects.Form;

namespace Microservice.Form.Application.Models
{
    public class FormAnswerFormMetaDataModel
    {
        public FormAnswerFormMetaDataModel()
        {
            // Do not remove this constructor. It is being used by JsonSerializer.Deserialize
        }

        public FormAnswerFormMetaDataModel(FormAnswerFormMetaData formAnswerFormMeta)
        {
            QuestionIdOrderList = formAnswerFormMeta.QuestionIdOrderList;
            FormQuestionOptionsOrderInfoList = formAnswerFormMeta.FormQuestionOptionsOrderInfoList?.Select(p => new FormQuestionOptionsOrderInfoModel(p));
        }

        public IEnumerable<Guid> QuestionIdOrderList { get; set; }

        public IEnumerable<FormQuestionOptionsOrderInfoModel> FormQuestionOptionsOrderInfoList { get; set; }
    }
}
