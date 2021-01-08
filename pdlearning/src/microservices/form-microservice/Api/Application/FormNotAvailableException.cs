using Thunder.Platform.Core.Exceptions;

namespace Microservice.Form.Application
{
    public class FormNotAvailableException : BusinessLogicException
    {
        public FormNotAvailableException() : base("The page you try to reach is not available.")
        {
        }
    }
}
