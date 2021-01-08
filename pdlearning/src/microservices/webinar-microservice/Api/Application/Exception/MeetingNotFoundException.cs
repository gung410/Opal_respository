using Thunder.Platform.Core.Exceptions;

namespace Microservice.Webinar.Application.Exception
{
    public class MeetingNotFoundException : BusinessLogicException
    {
        public MeetingNotFoundException()
                : base("The meeting does not exist.")
        {
        }
    }
}
