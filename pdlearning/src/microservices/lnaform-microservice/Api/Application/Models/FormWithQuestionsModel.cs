using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microservice.LnaForm.Domain.Entities;
using FormEntity = Microservice.LnaForm.Domain.Entities.Form;

namespace Microservice.LnaForm.Application.Models
{
    public class FormWithQuestionsModel
    {
        public FormWithQuestionsModel()
        {
            // Do not remove this constructor. It is being used by JsonSerializer.Deserialize
        }

        public FormWithQuestionsModel(FormEntity formEntity, IEnumerable<FormQuestion> formQuestionEntities, IEnumerable<FormSection> formSectionEntities, bool canUnpublishStandalone)
        {
            Form = new FormModel(formEntity, canUnpublishStandalone);
            FormQuestions = formQuestionEntities.Select(question =>
            {
                var questionModel = new FormQuestionModel(question);
                questionModel.QuestionTitle = HttpUtility.HtmlDecode(questionModel.QuestionTitle);
                return questionModel;
            });
            FormSections = formSectionEntities.Select(section => new FormSectionModel(section));
        }

        public FormModel Form { get; set; }

        public IEnumerable<FormQuestionModel> FormQuestions { get; set; }

        public IEnumerable<FormSectionModel> FormSections { get; set; }
    }
}
