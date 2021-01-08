using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace cxOrganization.Domain.HttpClients
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
            string payload = response.RequestMessage.Content != null ? await response.RequestMessage.Content.ReadAsStringAsync() : null;
            throw new cxHttpResponseException(response.StatusCode, content, payload, response.RequestMessage.RequestUri.ToString());
        }
    }

    public class cxHttpResponseException : Exception
    {
        public HttpStatusCode StatusCode { get; set; }
        public string RequestUri { get; set; }
        public string RequestPayload { get; set; }

        public cxHttpResponseException(HttpStatusCode statusCode, string content, string requestPayload = null, string requestUri = null) : base(content)
        {
            StatusCode = statusCode;
            RequestPayload = requestPayload;
            RequestUri = requestUri;
        }

        public override string ToString()
        {
            if (string.IsNullOrEmpty(RequestUri))
            {
                return ExceptionToString(this, null);
            }
            return ExceptionToString(this,
                description =>
                {
                    description.AppendFormat(
                        ", RequestUri={0}, RequestPayload='{1}'",
                        RequestUri,
                        RequestPayload);
                });
        }

        private string ExceptionToString(Exception ex,
        Action<StringBuilder> customFieldsFormatterAction)
        {
            StringBuilder description = new StringBuilder();
            description.AppendFormat("{0}: {1}", ex.GetType().Name, ex.Message);

            if (customFieldsFormatterAction != null)
            {
                customFieldsFormatterAction(description);
                description.Append(Environment.NewLine);
            }

            if (ex.InnerException != null)
            {
                description.AppendFormat(" ---> {0}", ex.InnerException);
                description.AppendFormat(
                    "{0}   --- End of inner exception stack trace ---{0}",
                    Environment.NewLine);
            }

            description.Append(ex.StackTrace);

            return description.ToString();
        }
    }
}
