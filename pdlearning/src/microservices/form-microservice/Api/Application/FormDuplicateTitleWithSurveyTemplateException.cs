using Thunder.Platform.Core.Exceptions;

namespace Microservice.Form.Application
{
    public class FormDuplicateTitleWithSurveyTemplateException : BusinessLogicException
    {
        public FormDuplicateTitleWithSurveyTemplateException() : base("Form title must not be duplicated with form survey templates.")
        {
        }
    }
}
