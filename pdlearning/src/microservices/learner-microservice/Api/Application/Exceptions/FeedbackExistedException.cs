using Thunder.Platform.Core.Exceptions;

namespace Microservice.Learner.Application.Exceptions
{
    public class FeedbackExistedException : BusinessLogicException
    {
        public FeedbackExistedException()
            : base("You can only provide the feedback once")
        {
        }
    }
}
