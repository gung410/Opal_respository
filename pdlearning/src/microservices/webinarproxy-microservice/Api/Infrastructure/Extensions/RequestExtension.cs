using System;
using System.Linq;
using Microsoft.AspNetCore.Http;

namespace Microservice.WebinarProxy.Infrastructure.Extensions
{
    public static class RequestExtension
    {
        public static bool IsJsonRequest(this HttpRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            var acceptHeader = request.Headers["Accept"].FirstOrDefault();
            var isJsonRequest = acceptHeader != null && acceptHeader.StartsWith("application/json", StringComparison.OrdinalIgnoreCase);
            if (isJsonRequest)
            {
                return true;
            }

            if (request.ContentType == null)
            {
                return false;
            }

            return request.ContentType.StartsWith("application/json", StringComparison.OrdinalIgnoreCase);
        }
    }
}
