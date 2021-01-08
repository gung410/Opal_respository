using Thunder.Platform.Core.Exceptions;

namespace Microservice.Learner.Application.Exceptions
{
    public class MyLearningPackageHasCompletedException : BusinessLogicException
    {
        public MyLearningPackageHasCompletedException()
            : base("SCORM result record has completed")
        {
        }
    }
}
