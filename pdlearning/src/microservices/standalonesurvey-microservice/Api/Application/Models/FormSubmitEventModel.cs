using System.Collections.Generic;
using Microservice.StandaloneSurvey.Domain.Entities;

namespace Microservice.StandaloneSurvey.Application.Models
{
    public class FormSubmitEventModel
    {
        public FormSubmitEventModel()
        {
        }

        public FormSubmitEventModel(SurveyAnswer surveyAnswer, IEnumerable<SurveyQuestionAnswer> formQuestionAnswers)
        {
            FormAnswer = surveyAnswer;
            FormQuestionAnswers = formQuestionAnswers;
        }

        public SurveyAnswer FormAnswer { get; set; }

        public IEnumerable<SurveyQuestionAnswer> FormQuestionAnswers { get; set; }
    }
}
