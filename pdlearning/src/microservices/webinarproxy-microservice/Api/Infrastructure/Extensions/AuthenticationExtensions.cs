using System;
using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;
using Microservice.WebinarProxy.Configurations;
using Microservice.WebinarProxy.Infrastructure.Caches;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Microservice.WebinarProxy.Infrastructure.Extensions
{
    public static class AuthenticationExtensions
    {
        public static IServiceCollection ConfigureOidc(this IServiceCollection services, IConfiguration configuration)
        {
            AuthenticationOptions authenticationOption = new AuthenticationOptions();
            configuration.GetSection(nameof(AuthenticationOptions)).Bind(authenticationOption);

            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            services.Configure<CookiePolicyOptions>(options =>
            {
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
                if (authenticationOption.SecureCookieAlways)
                {
                    options.Secure = CookieSecurePolicy.Always;
                }
            });

            services.AddTransient<ITicketStore, RedisCacheTicketStore>();

            services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
            })
            .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
            {
                var ticketStore = services.BuildServiceProvider().GetService<ITicketStore>();

                options.SessionStore = ticketStore;

                options.Events.OnSigningIn = ctx =>
                {
                    ctx.FilterGroupClaims();
                    return Task.CompletedTask;
                };
            })
            .AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
            {
                options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.SignedOutRedirectUri = authenticationOption.LogoutRedirectUrl;
                options.Authority = authenticationOption.IdmPublicUrl;
                options.RequireHttpsMetadata = false;
                options.ClientId = authenticationOption.OidcClientId;
                options.ClientSecret = authenticationOption.OidcClientSecret;
                options.CorrelationCookie.Path = authenticationOption.SigninOidcPath;
                options.NonceCookie.Path = authenticationOption.SigninOidcPath;
                options.SaveTokens = true;
                options.CallbackPath = new PathString(authenticationOption.SigninOidcPath);
                options.RemoteSignOutPath = new PathString(authenticationOption.SignOutOidcPath);

                options.Scope.Clear();
                foreach (string item in authenticationOption.ProxyAppScope)
                {
                    options.Scope.Add(item);
                }

                options.GetClaimsFromUserInfoEndpoint = true;
                options.ResponseType = "code";
                options.MetadataAddress = $"{authenticationOption.IdmInternalUrl}/.well-known/openid-configuration";

                options.Events = new OpenIdConnectEvents
                {
                    OnRedirectToIdentityProvider = redirectContext =>
                    {
                        // Force scheme of redirect URI (THE IMPORTANT PART)
                        redirectContext.ProtocolMessage.IssuerAddress = redirectContext
                        .ProtocolMessage
                        .IssuerAddress
                        .Replace(authenticationOption.IdmInternalUrl, authenticationOption.IdmPublicUrl);

                        redirectContext.ProtocolMessage.RedirectUri = authenticationOption.LoginRedirectUrl;

                        return Task.FromResult(0);
                    },
                    OnTicketReceived = context => Task.FromResult(0),
                    OnRedirectToIdentityProviderForSignOut = redirectContext =>
                    {
                        // Force scheme of redirect URI (THE IMPORTANT PART)
                        redirectContext.ProtocolMessage.IssuerAddress = redirectContext
                        .ProtocolMessage
                        .IssuerAddress
                        .Replace(authenticationOption.IdmInternalUrl, authenticationOption.IdmPublicUrl);

                        redirectContext.ProtocolMessage.PostLogoutRedirectUri = authenticationOption.LogoutRedirectUrl;

                        redirectContext.ProtocolMessage.RedirectUri = authenticationOption.LoginRedirectUrl;

                        return Task.FromResult(0);
                    }
                };
            });

            return services;
        }
    }
}
