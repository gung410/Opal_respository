using Thunder.Platform.Core.Exceptions;

namespace Microservice.Content.Application
{
    public class InvalidStatusException : BusinessLogicException
    {
        public InvalidStatusException()
            : base("Invalid status!")
        {
        }
    }
}
