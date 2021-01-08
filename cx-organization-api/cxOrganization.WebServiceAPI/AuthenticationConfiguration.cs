using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using cxOrganization.WebServiceAPI.Auth;
using IdentityModel.Client;
using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace cxOrganization.WebServiceAPI
{
    public static class AuthenticationConfiguration
    {
        public static void UseAuthenticationConfig(this IServiceCollection services,
            IConfiguration configuration)
        {
            // configure basic authentication 
            services.AddAuthentication(AuthenticationDefaults.SchemaNavigator)
                .AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>(AuthenticationDefaults.BasicAuthenticationScheme, null)
                .AddIdentityServerAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme, options =>
                {
                    var authority = configuration["AppSettings:AuthorityUrl"];
                    var internalURL = configuration["IDM_INTERNAL_URL"];
                    if (string.IsNullOrEmpty(internalURL))
                        internalURL = authority;
                    options.Authority = internalURL;
                    options.IntrospectionDiscoveryPolicy = new DiscoveryPolicy
                    {
                        Authority = internalURL,
                        ValidateIssuerName = false
                    };
                    options.RequireHttpsMetadata = false;
                    options.ApiName = configuration["Idm_ApiName"];
                    options.ApiSecret = configuration["Idm_ApiSecret"];
                    options.SupportedTokens = SupportedTokens.Both;
                    options.EnableCaching = true;
                    options.CacheDuration = TimeSpan.FromMinutes(10);
                })
                .AddPolicyScheme(AuthenticationDefaults.SchemaNavigator, "Bearer or Basic", options =>
                {
                    options.ForwardDefaultSelector = context =>
                    {
                        if (!context.Request.Headers.ContainsKey(AuthenticationDefaults.AuthorizationHeader))
                        {
                            return AuthenticationDefaults.BasicAuthenticationScheme;
                        }
                        var authHeader = AuthenticationHeaderValue.Parse(context.Request.Headers[AuthenticationDefaults.AuthorizationHeader]);
                        if (authHeader != null && authHeader.Scheme == JwtBearerDefaults.AuthenticationScheme)
                        {
                            return JwtBearerDefaults.AuthenticationScheme;
                        }
                        return AuthenticationDefaults.BasicAuthenticationScheme;
                    };
                });
            services.AddAuthorization(options =>
            {
                options.AddPolicy("AdminOnly", policy =>
                                  policy.RequireClaim(ClaimTypes.Name, configuration["AppSettings:AdminUsername"]));
            });
        }
    }
}
