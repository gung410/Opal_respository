using Thunder.Platform.Core.Exceptions;

namespace Microservice.WebinarAutoscaler.Application.Exception
{
    public class NoEC2ExistedInAutoScaleGroupException : BusinessLogicException
    {
        public NoEC2ExistedInAutoScaleGroupException()
            : base("No EC2 instances were existed in auto scale group.")
        {
        }
    }
}
