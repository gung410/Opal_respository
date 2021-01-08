using Thunder.Platform.Core.Exceptions;

namespace Microservice.Learner.Application.Exceptions
{
    public class InValidLearningException : BusinessLogicException
    {
        public InValidLearningException()
            : base("The class run is must be started and not finished.")
        {
        }
    }
}
