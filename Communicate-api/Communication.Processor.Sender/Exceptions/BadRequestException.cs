using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Communication.Processor.Sender.Exceptions
{
    public class BadRequestException : ApplicationBaseException
    {
        public BadRequestException(string message, ApplicationErrorCodes errorCode, HttpStatusCode statusCode = HttpStatusCode.BadRequest) : base(message, errorCode, statusCode)
        {
        }
    }
}
