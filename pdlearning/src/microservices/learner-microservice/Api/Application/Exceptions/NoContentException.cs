using Thunder.Platform.Core.Exceptions;

namespace Microservice.Learner.Application.Exceptions
{
    public class NoContentException : BusinessLogicException
    {
        public NoContentException() : base("There is no content.")
        {
        }
    }
}
