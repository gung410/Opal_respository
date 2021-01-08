using Thunder.Platform.Core.Exceptions;

namespace Microservice.Learner.Application.Exceptions
{
    public class MyCourseExistedException : BusinessLogicException
    {
        public MyCourseExistedException()
            : base("My Course existed")
        {
        }
    }
}
