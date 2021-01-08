using Thunder.Platform.Core.Exceptions;

namespace Microservice.LnaForm.Application
{
    public class FormStandaloneAccessDeniedException : BusinessLogicException
    {
        public FormStandaloneAccessDeniedException() : base("You are not assigned to this form. Please contact your administrator.")
        {
        }
    }
}
