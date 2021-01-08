using Thunder.Platform.Core.Exceptions;

namespace Microservice.Learner.Application.Exceptions
{
    public class UserBookmarkExistedException : BusinessLogicException
    {
        public UserBookmarkExistedException()
            : base("Your bookmarked has already")
        {
        }
    }
}
