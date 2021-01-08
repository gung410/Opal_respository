using Thunder.Platform.Core.Exceptions;

namespace Microservice.WebinarAutoscaler.Application.Exception
{
    public class RulePriorityNoSpacingException : BusinessLogicException
    {
        public RulePriorityNoSpacingException()
            : base("Rule priority is accommodated from 1 to 5000.")
        {
        }
    }
}
