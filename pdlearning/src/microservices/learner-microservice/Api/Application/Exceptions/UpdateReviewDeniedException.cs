using Thunder.Platform.Core.Exceptions;

namespace Microservice.Learner.Application.Exceptions
{
    public class UpdateReviewDeniedException : BusinessLogicException
    {
        public UpdateReviewDeniedException() : base("You cannot update this your review.")
        {
        }
    }
}
