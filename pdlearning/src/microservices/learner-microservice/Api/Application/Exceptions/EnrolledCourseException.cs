using Thunder.Platform.Core.Exceptions;

namespace Microservice.Learner.Application.Exceptions
{
    public class EnrolledCourseException : BusinessLogicException
    {
        public EnrolledCourseException()
            : base("You have already enrolled in this course.")
        {
        }
    }
}
