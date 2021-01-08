using System;

namespace cxOrganization.Domain.Exceptions
{
    public class CxException : Exception
    {
        public CxException()
        {
        }

        public CxException(string message) : base(message)
        {
        }

        public CxException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
