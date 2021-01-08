using Thunder.Platform.Core.Exceptions;

namespace Microservice.Calendar.Application.Exceptions
{
    public class DeleteEventDeniedException : BusinessLogicException
    {
        public DeleteEventDeniedException() : base("You do not have permission to delete event for the community")
        {
        }
    }
}
