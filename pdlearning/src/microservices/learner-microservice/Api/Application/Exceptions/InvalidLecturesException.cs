using Thunder.Platform.Core.Exceptions;

namespace Microservice.Learner.Application.Exceptions
{
    public class InvalidLecturesException : BusinessLogicException
    {
        public InvalidLecturesException()
            : base("LectureId in a Course must be unique")
        {
        }
    }
}
