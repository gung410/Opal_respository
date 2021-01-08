using System;
using FluentValidation.AspNetCore;
using IdentityModel.Client;
using IdentityServer4.AccessTokenValidation;
using Microservice.WebinarVideoConverter.Configuration;
using Microservice.WebinarVideoConverter.Infrastructure;
using Microservice.WebinarVideoConverter.Infrastructure.Configurations;
using Microservice.WebinarVideoConverter.Infrastructure.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Thunder.Platform.AspNetCore;
using Thunder.Platform.AspNetCore.Cors;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;
using Thunder.Platform.EntityFrameworkCore;
using Thunder.Service.Authentication;

namespace Microservice.WebinarVideoConverter
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string ApiName { get; } = "OPAL2.0 - Webinar Video Converter API";

        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddMvc(options =>
                {
                    options.Filters.Add<AuthorizationFilter>();
                })
                .AddFluentValidation()
                .AddControllersAsServices()
                .AddThunderJsonOptions()
                .SetCompatibilityVersion(CompatibilityVersion.Latest)
                .Services
                .AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddIdentityServerAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme, options =>
                {
                    var authority = _configuration["AuthSettings:AuthorityUrl"];

                    // Note: internal url required https cause issue. Temporary comment out it
                    var internalUrl = _configuration["OpalAuthorityInternalUrl"];

                    if (string.IsNullOrEmpty(internalUrl))
                    {
                        internalUrl = authority;
                    }

                    options.RequireHttpsMetadata = false;
                    options.Authority = internalUrl;
                    options.IntrospectionDiscoveryPolicy = new DiscoveryPolicy
                    {
                        Authority = internalUrl,
                        ValidateIssuerName = false
                    };
                    options.ApiName = _configuration["AuthSettings:IdmClientId"];
                    options.ApiSecret = _configuration["AuthSettings:IdmClientSecret"];
                    options.SupportedTokens = SupportedTokens.Both;
                    options.EnableCaching = true;
                    options.CacheDuration = TimeSpan.FromMinutes(10);
                })
                .Services
                .AddThunderCors()
                .AddThunderSwagger(ApiName, "v1")
                .AddAwsClient(_configuration)
                .AddOptions<PlaybackOptions>()
                .Bind(_configuration.GetSection(nameof(PlaybackOptions)));

            services
                .Configure<MvcOptions>(mvcOptions =>
                {
                    mvcOptions.AddThunderMvcOptions();
                });

            services
                .Configure<RecordingConvertOptions>(_configuration.GetSection(nameof(RecordingConvertOptions)))
                .Configure<AmazonS3Options>(_configuration.GetSection(nameof(AmazonS3Options)))
                .Configure<FailedRetryOptions>(_configuration.GetSection(nameof(FailedRetryOptions)))
                .Configure<ConverterTaskOptions>(_configuration.GetSection(nameof(ConverterTaskOptions)));
        }

        public void Configure(
            IApplicationBuilder app,
            IDbContextResolver dbContextResolver,
            IConnectionStringResolver connectionStringResolver)
        {
            dbContextResolver.InitDatabase<WebinarRecordMangementDbContext>(
                connectionStringResolver.GetNameOrConnectionString(new ConnectionStringResolveArgs()));

            app.UseThunderExceptionHandler();
            app.UserThunderRequestIdGenerator();
            app.UserThunderSecurityHeaders();

            app.UseMiddleware<ThunderAuthenticationMiddleware>();
            app.UseRouting();
            app.UseThunderCors();
            app.UseThunderUnitOfWork();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseThunderSwagger(ApiName, "v1");
        }
    }
}
