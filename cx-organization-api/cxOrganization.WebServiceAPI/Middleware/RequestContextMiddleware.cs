using System;
using System.Threading.Tasks;
using cxPlatform.Core.Extentions.Request;
using Microsoft.AspNetCore.Http;

namespace cxOrganization.WebServiceAPI.Middlewares
{
    public class RequestContextMiddleware
    {
        private readonly RequestDelegate next;

        public RequestContextMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            var requestId = GenerateRequestIdInContext(context, out string xArrLogId);
            var correlationId = GenerateCorrelationIdInContext(context);

            SetRequestContentToResponse(context, requestId, xArrLogId, correlationId);

            await next(context);
        }

        private static void SetRequestContentToResponse(HttpContext context, string requestId, string xArrLogId, string correlationId)
        {
            context.Response.Headers.Add(HeaderKeys.RequestId, requestId);
            context.Response.Headers.Add(HeaderKeys.CorrelationId, correlationId);

            if (!string.IsNullOrEmpty(xArrLogId))
            {
                context.Response.Headers.Add(HeaderKeys.XArrLogId, xArrLogId);
            }
        }

        private static string GenerateRequestIdInContext(HttpContext context, out string xArrLogId)
        {
            var request = context.Request;
            xArrLogId = request.GetXArrLogId();

            var requestId = string.IsNullOrEmpty(xArrLogId)
                ? Guid.NewGuid().ToString() : xArrLogId;
            var headerRequestId = request.GetRequestId();
            if (!string.IsNullOrEmpty(headerRequestId))
            {
                request.RemoveHeaderValue(HeaderKeys.RequestId);
            }
            request.Headers.Add(HeaderKeys.RequestId, requestId);
            return requestId;
        }
        private static string GenerateCorrelationIdInContext(HttpContext context)
        {
            var headerCorrelationId = context.Request.GetCustomCorrelationId();
            if (string.IsNullOrEmpty(headerCorrelationId))
            {
                headerCorrelationId = Guid.NewGuid().ToString();
                context.Request.Headers.Add(HeaderKeys.CorrelationId, headerCorrelationId);
            }
            return headerCorrelationId;
        }
    }
}
