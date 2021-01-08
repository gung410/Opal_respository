using Thunder.Platform.Core.Exceptions;

namespace Microservice.Webinar.Application.Exception
{
    public class BookingNotFoundException : BusinessLogicException
    {
        public BookingNotFoundException()
            : base("The booking does not exist.")
        {
        }
    }
}
