using Thunder.Platform.Core.Exceptions;

namespace Microservice.Uploader.Application
{
    public class PersonalSpaceAccessDeniedException : BusinessLogicException
    {
        public PersonalSpaceAccessDeniedException()
            : base("You don't have permission to access this personal space.")
        {
        }
    }
}
