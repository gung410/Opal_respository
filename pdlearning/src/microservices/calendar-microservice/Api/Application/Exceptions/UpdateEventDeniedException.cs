using Thunder.Platform.Core.Exceptions;

namespace Microservice.Calendar.Application.Exceptions
{
    public class UpdateEventDeniedException : BusinessLogicException
    {
        public UpdateEventDeniedException() : base("You do not have permission to update the event.")
        {
        }
    }
}
