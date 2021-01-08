using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microservice.StandaloneSurvey.Domain.Entities;

namespace Microservice.StandaloneSurvey.Application.Models
{
    public class SurveyWithQuestionsModel
    {
        public SurveyWithQuestionsModel()
        {
            // Do not remove this constructor. It is being used by JsonSerializer.Deserialize
        }

        public SurveyWithQuestionsModel(Domain.Entities.StandaloneSurvey formEntity, IEnumerable<SurveyQuestion> formQuestionEntities, IEnumerable<SurveySection> formSectionEntities, bool canUnpublishStandalone)
        {
            Form = new StandaloneSurveyModel(formEntity, canUnpublishStandalone);
            FormQuestions = formQuestionEntities.Select(question =>
            {
                var questionModel = new SurveyQuestionModel(question);
                questionModel.QuestionTitle = HttpUtility.HtmlDecode(questionModel.QuestionTitle);
                return questionModel;
            });
            FormSections = formSectionEntities.Select(section => new SurveySectionModel(section));
        }

        public StandaloneSurveyModel Form { get; set; }

        public IEnumerable<SurveyQuestionModel> FormQuestions { get; set; }

        public IEnumerable<SurveySectionModel> FormSections { get; set; }
    }
}
