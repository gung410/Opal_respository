using Thunder.Platform.Core.Exceptions;

namespace Microservice.Form.Application
{
    public class QuestionBankAccessDeniedException : BusinessLogicException
    {
        public QuestionBankAccessDeniedException() : base("You do not have the permission to access this question bank.")
        {
        }
    }
}
