using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microservice.WebinarProxy.Application.Constants;
using Microservice.WebinarProxy.Application.Services;
using Microservice.WebinarProxy.Configurations;
using Microservice.WebinarProxy.Infrastructure.Extensions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using AuthenticationOptions = Microservice.WebinarProxy.Configurations.AuthenticationOptions;

namespace Microservice.WebinarProxy.Middleware
{
    public class AuthorizationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IChecksumHelper _opal2CheckSum;
        private readonly AuthenticationOptions _authOption;
        private readonly ProxyOptions _proxyOption;

        public AuthorizationMiddleware(
            RequestDelegate next,
            IChecksumHelper opal2CheckSum,
            IOptions<ProxyOptions> proxyOptions,
            IOptions<AuthenticationOptions> authOptions)
        {
            _next = next;
            _opal2CheckSum = opal2CheckSum;
            _authOption = authOptions.Value;
            _proxyOption = proxyOptions.Value;
        }

        public async Task Invoke(
            HttpContext context,
            ISessionValidator sessionValidator,
            IMeetingUrlHelper urlHelper,
            ILogger<AuthorizationMiddleware> logger)
        {
            if (_authOption.IgnoreAuthenticationPaths.Contains(context.Request.Path))
            {
                await _next.Invoke(context);
                return;
            }

            if (!context.Request.Path.Value.StartsWith("/session/api/join"))
            {
                var redirectUrl = urlHelper.BuildErrorUrl(_proxyOption.DefaultFallbackUrl, ErrorCodes.InternalServerError);
                context.Response.Redirect(redirectUrl);
                return;
            }

            var clientChecksum = _opal2CheckSum.GetChecksum(context.Request.Query);

            if (!_opal2CheckSum.ValidateClientChecksum(context.Request.Query, clientChecksum))
            {
                var redirectUrl = urlHelper.BuildErrorUrl(_proxyOption.DefaultFallbackUrl, ErrorCodes.UnauthenticatedUser);
                context.Response.Redirect(redirectUrl);
                return;
            }

            var token = await context.GetTokenAsync("access_token");
            var isValidSession = await sessionValidator.ValidateByToken(token);
            if (!isValidSession)
            {
                if (_authOption.UseAuthenticatedForJsonRequest && context.Request.IsJsonRequest())
                {
                    context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    return;
                }

                await context.SignOutCookieAndChallengeAgain();
                return;
            }

            var userId = context.User.Claims?.FirstOrDefault(t => t.Type == "sub");
            var requestUserId = context.Request.Query["userID"];
            logger.LogInformation($"UserId: {userId}, RequestId: {requestUserId}");

            if (!string.Equals(requestUserId, userId?.Value, StringComparison.OrdinalIgnoreCase))
            {
                await context.SignOutCookieAndChallengeAgain();
                return;
            }

            await _next.Invoke(context);
        }
    }
}
