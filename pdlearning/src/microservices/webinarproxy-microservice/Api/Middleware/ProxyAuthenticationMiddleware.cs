using System.Net;
using System.Threading.Tasks;
using Microservice.WebinarProxy.Configurations;
using Microservice.WebinarProxy.Infrastructure.Extensions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using AuthenticationOptions = Microservice.WebinarProxy.Configurations.AuthenticationOptions;

namespace Microservice.WebinarProxy.Middleware
{
    public class ProxyAuthenticationMiddleware
    {
        private readonly RequestDelegate _next;

        public ProxyAuthenticationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(
            HttpContext context,
            IOptions<AuthenticationOptions> authOptions,
            ILogger<ProxyAuthenticationMiddleware> logger)
        {
            var authOption = authOptions.Value;
            logger.LogInformation("IsAuthenticated: {IsAuthenticated}", context.User.Identity.IsAuthenticated);
            if (!authOption.IgnoreAuthenticationPaths.Contains(context.Request.Path) && !context.User.Identity.IsAuthenticated)
            {
                logger.LogInformation("Challenge: {IsAuthenticated}", context.User.Identity.IsAuthenticated);

                if (authOption.UseAuthenticatedForJsonRequest && context.Request.IsJsonRequest())
                {
                    context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    return;
                }

                await context.ChallengeAsync(OpenIdConnectDefaults.AuthenticationScheme);
                return;
            }

            await _next.Invoke(context);
        }
    }
}
