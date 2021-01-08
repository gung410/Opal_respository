using Thunder.Platform.Core.Exceptions;

namespace Microservice.Learner.Application.Exceptions
{
    public class LectureDuplicatedException : BusinessLogicException
    {
        public LectureDuplicatedException()
            : base("Lecture duplicated")
        {
        }
    }
}
