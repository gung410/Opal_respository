using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Http;

namespace Microservice.WebinarProxy.Infrastructure.Extensions
{
    public static class HttpContextExtension
    {
        public static async Task SignOutCookieAndChallengeAgain(this HttpContext context)
        {
            var authProp = new AuthenticationProperties
            {
                RedirectUri = $"/"
            };
            await context.SignOutAsync(OpenIdConnectDefaults.AuthenticationScheme, authProp);
            await context.ChallengeAsync(OpenIdConnectDefaults.AuthenticationScheme);
        }
    }
}
