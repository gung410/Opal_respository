using Thunder.Platform.Core.Exceptions;

namespace Microservice.Learner.Application.Exceptions
{
    public class UserTrackingActivityNotSupportEvent : BusinessLogicException
    {
        public UserTrackingActivityNotSupportEvent()
            : base("Event was not supported.")
        {
        }
    }
}
