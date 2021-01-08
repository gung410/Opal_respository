using Thunder.Platform.Core.Exceptions;

namespace Microservice.Learner.Application.Exceptions
{
    public class StatusInvalidException : BusinessLogicException
    {
        public StatusInvalidException() : base("Status is invalid.")
        {
        }
    }
}
