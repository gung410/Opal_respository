using System;

namespace Microservice.Learner.Application.Exceptions
{
    /// <summary>
    /// Inherited from <see cref="NotSupportedException"/>.
    /// </summary>
    public class UnexpectedStatusException : NotSupportedException
    {
        public UnexpectedStatusException(string status)
            : base($"Unexpected with status {status}")
        {
        }
    }
}
