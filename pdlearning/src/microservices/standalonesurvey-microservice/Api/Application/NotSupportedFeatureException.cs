using Thunder.Platform.Core.Exceptions;

namespace Microservice.StandaloneSurvey.Application
{
    public class NotSupportedFeatureException : BusinessLogicException
    {
        public NotSupportedFeatureException() : base("The module did not intend to support this feature.")
        {
        }
    }
}
