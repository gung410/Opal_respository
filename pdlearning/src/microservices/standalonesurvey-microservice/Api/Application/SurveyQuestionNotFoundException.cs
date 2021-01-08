using Thunder.Platform.Core.Exceptions;

namespace Microservice.StandaloneSurvey.Application
{
    public class SurveyQuestionNotFoundException : BusinessLogicException
    {
        public SurveyQuestionNotFoundException() : base("Could not find the form question.")
        {
        }
    }
}
