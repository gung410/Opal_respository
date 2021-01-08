using Thunder.Platform.Core.Exceptions;

namespace Microservice.Form.Application
{
    public class FormAccessDeniedException : BusinessLogicException
    {
        public FormAccessDeniedException() : base("You do not have the permission to access this form.")
        {
        }
    }
}
