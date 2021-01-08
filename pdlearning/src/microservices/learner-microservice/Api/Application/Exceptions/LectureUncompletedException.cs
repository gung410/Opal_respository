using Thunder.Platform.Core.Exceptions;

namespace Microservice.Learner.Application.Exceptions
{
    public class LectureUncompletedException : BusinessLogicException
    {
        public LectureUncompletedException()
            : base("Course has uncompleted lecture(s)")
        {
        }
    }
}
