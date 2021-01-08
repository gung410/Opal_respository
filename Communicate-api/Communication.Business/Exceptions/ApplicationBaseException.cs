using Newtonsoft.Json;
using System;
using System.Net;

namespace Communication.Business.Exceptions
{
    public class ApplicationBaseException : Exception
    {
        public HttpStatusCode StatusCode { get; set; }
        public ApplicationErrorCodes ErrorCode { get; set; }
        public ApplicationBaseException(string message, ApplicationErrorCodes errorCode, HttpStatusCode statusCode) : base(message)
        {
            StatusCode = statusCode;
            ErrorCode = errorCode;
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(new { StatusCode, ErrorCode, Message },  new Newtonsoft.Json.Converters.StringEnumConverter());
        }
    }
}
