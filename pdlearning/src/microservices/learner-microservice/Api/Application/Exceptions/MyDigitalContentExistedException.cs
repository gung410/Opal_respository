using Thunder.Platform.Core.Exceptions;

namespace Microservice.Learner.Application.Exceptions
{
    public class MyDigitalContentExistedException : BusinessLogicException
    {
        public MyDigitalContentExistedException()
            : base("My Digital Content existed")
        {
        }
    }
}
