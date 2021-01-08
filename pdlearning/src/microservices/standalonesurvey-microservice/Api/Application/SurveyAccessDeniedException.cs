using Thunder.Platform.Core.Exceptions;

namespace Microservice.StandaloneSurvey.Application
{
    public class SurveyAccessDeniedException : BusinessLogicException
    {
        public SurveyAccessDeniedException() : base("You do not have the permission to access this survey.")
        {
        }
    }
}
