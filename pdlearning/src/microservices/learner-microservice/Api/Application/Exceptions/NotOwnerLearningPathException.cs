using Thunder.Platform.Core.Exceptions;

namespace Microservice.Learner.Application.Exceptions
{
    public class NotOwnerLearningPathException : BusinessLogicException
    {
        public NotOwnerLearningPathException() : base("You aren't owner of this learning path.")
        {
        }
    }
}
