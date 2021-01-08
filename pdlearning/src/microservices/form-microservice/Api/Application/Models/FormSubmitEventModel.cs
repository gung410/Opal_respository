using System.Collections.Generic;
using Microservice.Form.Domain.Entities;

namespace Microservice.Form.Application.Models
{
    public class FormSubmitEventModel
    {
        public FormSubmitEventModel()
        {
        }

        public FormSubmitEventModel(FormAnswer formAnswer, IEnumerable<FormQuestionAnswer> formQuestionAnswers)
        {
            FormAnswer = formAnswer;
            FormQuestionAnswers = formQuestionAnswers;
        }

        public FormAnswer FormAnswer { get; set; }

        public IEnumerable<FormQuestionAnswer> FormQuestionAnswers { get; set; }
    }
}
