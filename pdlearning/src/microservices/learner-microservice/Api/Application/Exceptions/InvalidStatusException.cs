using Thunder.Platform.Core.Exceptions;

namespace Microservice.Learner.Application.Exceptions
{
    public class InvalidStatusException : BusinessLogicException
    {
        public InvalidStatusException() : base("Status is invalid.")
        {
        }
    }
}
