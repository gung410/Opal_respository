using Thunder.Platform.Core.Exceptions;

namespace Microservice.Learner.Application.Exceptions
{
    public class LearningPathNotFoundException : BusinessLogicException
    {
        public LearningPathNotFoundException()
            : base("Learning path not found")
        {
        }
    }
}
