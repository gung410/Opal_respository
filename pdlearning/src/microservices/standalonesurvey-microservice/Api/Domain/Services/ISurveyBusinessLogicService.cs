using System.Collections.Generic;
using Microservice.StandaloneSurvey.Domain.Entities;

namespace Microservice.StandaloneSurvey.Domain.Services
{
    public interface ISurveyBusinessLogicService
    {
        void EnsureFormQuestionsValidToSave(Entities.StandaloneSurvey form, IEnumerable<SurveyQuestion> formQuestions, IEnumerable<SurveySection> formSections);
    }
}
