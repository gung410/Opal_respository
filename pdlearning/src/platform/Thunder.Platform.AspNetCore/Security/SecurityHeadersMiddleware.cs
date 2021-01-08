using System;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Http;
using Thunder.Platform.Core;
using Thunder.Platform.Core.Context;

namespace Thunder.Platform.AspNetCore.Security
{
    public class SecurityHeadersMiddleware
    {
        private readonly RequestDelegate _next;

        public SecurityHeadersMiddleware(RequestDelegate next)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
        }

        public Task Invoke([NotNull] HttpContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            // apply the request ID to the response header for client side tracking
            context.Response.OnStarting(() =>
            {
                context.Response.Headers.Add(CommonHttpHeaderNames.XssProtection, "1; mode=block");
                return Task.CompletedTask;
            });

            return _next(context);
        }
    }
}
