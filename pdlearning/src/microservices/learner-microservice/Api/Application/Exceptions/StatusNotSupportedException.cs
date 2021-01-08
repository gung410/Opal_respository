using System;

namespace Microservice.Learner.Application.Exceptions
{
    /// <summary>
    /// Inherited from <see cref="NotSupportedException"/>.
    /// </summary>
    public class StatusNotSupportedException : NotSupportedException
    {
        public StatusNotSupportedException(string status)
            : base($"Unexpected with status {status}")
        {
        }
    }
}
