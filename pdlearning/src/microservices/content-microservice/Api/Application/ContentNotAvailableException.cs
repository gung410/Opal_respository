using Thunder.Platform.Core.Exceptions;

namespace Microservice.Content.Application
{
    public class ContentNotAvailableException : BusinessLogicException
    {
        public ContentNotAvailableException()
            : base("The page you try to reach is not available.")
        {
        }
    }
}
