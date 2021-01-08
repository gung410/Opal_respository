using Thunder.Platform.Core.Exceptions;

namespace Microservice.Learner.Application.Exceptions
{
    public class MyClassRunExistedException : BusinessLogicException
    {
        public MyClassRunExistedException()
            : base("My ClassRun existed")
        {
        }
    }
}
