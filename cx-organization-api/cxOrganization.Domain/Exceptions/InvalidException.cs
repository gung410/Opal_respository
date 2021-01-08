using System;
using System.Collections.Generic;
using System.Text;

namespace cxOrganization.Domain.Exceptions
{
    public class InvalidException : CxException
    {
        public InvalidException()
        {
        }

        public InvalidException(string message) : base(message)
        {
        }

        public InvalidException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
