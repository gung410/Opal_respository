using Thunder.Platform.Core.Exceptions;

namespace Microservice.StandaloneSurvey.Application
{
    public class MaximumAttemptReachedException : BusinessLogicException
    {
        public MaximumAttemptReachedException() : base("You have reached the maximum number of attempts for this form")
        {
        }
    }
}
