using Thunder.Platform.Core.Exceptions;

namespace Microservice.Learner.Application.Exceptions
{
    public class NotSupportedUserTrackingEventException : BusinessLogicException
    {
        public NotSupportedUserTrackingEventException() : base("This User Tracking Event was not supported.")
        {
        }
    }
}
