using Thunder.Platform.Core.Exceptions;

namespace Microservice.Calendar.Application.Exceptions
{
    public class CreateEventDeniedException : BusinessLogicException
    {
        public CreateEventDeniedException() : base("You do not have permission to create event for the community.")
        {
        }
    }
}
