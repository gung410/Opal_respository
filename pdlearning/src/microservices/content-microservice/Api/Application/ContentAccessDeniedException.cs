using Thunder.Platform.Core.Exceptions;

namespace Microservice.Content.Application
{
    public class ContentAccessDeniedException : BusinessLogicException
    {
        public ContentAccessDeniedException()
            : base("You don't have permission to access this content.")
        {
        }
    }
}
