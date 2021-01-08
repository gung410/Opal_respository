using Thunder.Platform.Core.Exceptions;

namespace Microservice.Uploader.Application
{
    public class PersonalFileAccessDeniedException : BusinessLogicException
    {
        public PersonalFileAccessDeniedException()
            : base("You don't have permission to access this personal file.")
        {
        }
    }
}
