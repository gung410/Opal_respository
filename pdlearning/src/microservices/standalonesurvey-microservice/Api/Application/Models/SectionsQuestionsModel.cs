using System.Collections.Generic;

namespace Microservice.StandaloneSurvey.Application.Models
{
    public class SectionsQuestionsModel
    {
        public List<SurveyQuestionModel> FormQuestions { get; set; }

        public List<SurveySectionModel> FormSections { get; set; }
    }
}
