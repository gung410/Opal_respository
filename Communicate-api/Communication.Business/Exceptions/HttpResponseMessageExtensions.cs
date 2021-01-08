using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Communication.Business.Exceptions
{
    public static class HttpResponseMessageExtensions
    {
        public static async Task<HttpContent> EnsureSuccessStatusCodeAsync(this Task<HttpResponseMessage> responseTask)
        {
            var response = await responseTask;
            if (response.IsSuccessStatusCode)
            {
                return response.Content;
            }

            var content = await response.Content.ReadAsStringAsync();

            if (response.Content != null)
                response.Content.Dispose();

            throw new cxHttpResponseException(response.StatusCode, content);
        }
    }

    public class cxHttpResponseException : Exception
    {
        public HttpStatusCode StatusCode { get; private set; }

        public cxHttpResponseException(HttpStatusCode statusCode, string content) : base(content)
        {
            StatusCode = statusCode;
        }
    }
}
