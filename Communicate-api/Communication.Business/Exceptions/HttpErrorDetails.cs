using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Communication.Business.Exceptions
{
    public class HttpErrorDetails
    {
        public HttpStatusCode StatusCode { get; set; }
        public string Message { get; set; }
        public string ErrorSource { get; set; }
        public string Detail { get; set; }
    }
}
