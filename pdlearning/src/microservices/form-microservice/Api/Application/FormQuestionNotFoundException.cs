using Thunder.Platform.Core.Exceptions;

namespace Microservice.Form.Application
{
    public class FormQuestionNotFoundException : BusinessLogicException
    {
        public FormQuestionNotFoundException() : base("Could not find the form question.")
        {
        }
    }
}
