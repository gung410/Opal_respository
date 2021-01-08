using Thunder.Platform.Core.Exceptions;

namespace Microservice.Learner.Application.Exceptions
{
    public class NoContentChangeException : BusinessLogicException
    {
        public NoContentChangeException()
            : base("No content changes.")
        {
        }
    }
}
