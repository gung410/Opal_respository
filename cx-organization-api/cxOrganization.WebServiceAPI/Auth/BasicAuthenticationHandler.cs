using System;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using cxOrganization.Domain.Settings;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace cxOrganization.WebServiceAPI.Auth
{
    public class BasicAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private readonly AppSettings _appSettings;
        public BasicAuthenticationHandler(
            IOptions<AppSettings> appSettingOption,
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock)
            : base(options, logger, encoder, clock)
        {
            _appSettings = appSettingOption.Value;
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            await Task.CompletedTask;
            if (!Request.Headers.ContainsKey(AuthenticationDefaults.AuthorizationHeader))
                return AuthenticateResult.Fail("Missing Authorization Header");
            try
            {
                var authHeader = AuthenticationHeaderValue.Parse(Request.Headers[AuthenticationDefaults.AuthorizationHeader]);
                if (authHeader.Scheme != AuthenticationDefaults.BasicAuthenticationScheme)
                    return null;
                var credentialBytes = Convert.FromBase64String(authHeader.Parameter);
                var credentials = Encoding.UTF8.GetString(credentialBytes).Split(':');
                var username = credentials[0];
                var password = credentials[1];
                if((_appSettings.ServiceUsername == username && _appSettings.ServicePassword == password)
                    || (_appSettings.AdminUsername == username && _appSettings.AdminPassword == password))
                {
                    var identity = new GenericIdentity(username);
                    var ticket = new AuthenticationTicket(new ClaimsPrincipal(identity), Scheme.Name);
                    return AuthenticateResult.Success(ticket);
                }
                else
                {
                    return null;
                }
            }
            catch
            {
                return AuthenticateResult.Fail("Invalid Authorization Header");
            }
        }
    }
}
