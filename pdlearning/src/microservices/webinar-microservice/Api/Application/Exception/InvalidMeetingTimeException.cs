using Thunder.Platform.Core.Exceptions;

namespace Microservice.Webinar.Application.Exception
{
    public class InvalidMeetingTimeException : BusinessLogicException
    {
        public InvalidMeetingTimeException()
            : base("The meeting is not available for now.")
        {
        }
    }
}
