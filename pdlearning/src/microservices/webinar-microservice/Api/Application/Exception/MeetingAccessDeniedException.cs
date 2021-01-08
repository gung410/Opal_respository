using Thunder.Platform.Core.Exceptions;

namespace Microservice.Webinar.Application.Exception
{
    public class MeetingAccessDeniedException : BusinessLogicException
    {
        public MeetingAccessDeniedException()
            : base("You do not have permission to access the meeting.")
        {
        }
    }
}
