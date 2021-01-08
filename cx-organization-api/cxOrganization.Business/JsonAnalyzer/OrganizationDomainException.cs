using System;
using System.Net;
using System.Runtime.Serialization;

namespace cxOrganization.Business.JsonAnalyzer
{
    [Serializable]
    public class OrganizationDomainException : Exception
    {
        public OrganizationDomainException()
        {
        }

        public HttpStatusCode ErrorCode { get; set; }

        public OrganizationDomainException(HttpStatusCode errorCode, string message) : base(message)
        {
            ErrorCode = errorCode;
        }

        public OrganizationDomainException(string message) : base(message)
        {
        }

        public OrganizationDomainException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected OrganizationDomainException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}