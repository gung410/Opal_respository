using Thunder.Platform.Core.Exceptions;

namespace Microservice.StandaloneSurvey.Application
{
    public class ResponseNotFoundException : BusinessLogicException
    {
        public ResponseNotFoundException() : base("Response was not found.")
        {
        }
    }
}
