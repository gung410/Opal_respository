using Thunder.Platform.Core.Exceptions;

namespace Microservice.StandaloneSurvey.Application
{
    public class StandaloneSurveyAccessDeniedException : BusinessLogicException
    {
        public StandaloneSurveyAccessDeniedException() : base("You are not assigned to this survey. Please contact your administrator.")
        {
        }
    }
}
