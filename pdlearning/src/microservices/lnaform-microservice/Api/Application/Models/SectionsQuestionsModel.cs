using System.Collections.Generic;

namespace Microservice.LnaForm.Application.Models
{
    public class SectionsQuestionsModel
    {
        public List<FormQuestionModel> FormQuestions { get; set; }

        public List<FormSectionModel> FormSections { get; set; }
    }
}
