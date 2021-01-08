using System;
using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using FluentValidation.AspNetCore;
using IdentityModel.Client;
using IdentityServer4.AccessTokenValidation;
using Microservice.Uploader.Infrastructure;
using Microservice.Uploader.Options;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Thunder.Platform.AspNetCore;
using Thunder.Platform.AspNetCore.Cors;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;
using Thunder.Platform.EntityFrameworkCore;
using Thunder.Service.Authentication;

namespace Microservice.Uploader
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string ApiName { get; } = "OPAL2.0 - Uploader API";

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

                    // var internalUrl = string.Empty;
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
                .AddThunderSwagger(ApiName, "v1");

            services
                .Configure<MvcOptions>(mvcOptions =>
                {
                    mvcOptions.AddThunderMvcOptions();
                })
                .AddOptions<AmazonS3Options>().Bind(_configuration.GetSection(nameof(AmazonS3Options)))
                .Services.AddOptions<AcceptanceOptions>().Bind(_configuration.GetSection(nameof(AcceptanceOptions)))
                .Services.AddOptions<AllowedFilesOptions>().Bind(_configuration.GetSection(nameof(AllowedFilesOptions)))
                .Services.AddOptions<ScormProcessingOptions>().Bind(_configuration.GetSection(nameof(ScormProcessingOptions)));

            services.AddTransient<IAmazonS3>(provider =>
            {
                var s3Options = provider.GetRequiredService<IOptions<AmazonS3Options>>().Value;
                return new AmazonS3Client(
                    new BasicAWSCredentials(s3Options.AccessKey, s3Options.SecretKey),
                    RegionEndpoint.GetBySystemName(s3Options.Region));
            });
        }

        public void Configure(
            IApplicationBuilder app,
            IDbContextResolver dbContextResolver,
            IConnectionStringResolver connectionStringResolver)
        {
            dbContextResolver.InitDatabase<UploaderDbContext>(
               connectionStringResolver.GetNameOrConnectionString(new ConnectionStringResolveArgs()));

            app.UseThunderExceptionHandler();
            app.UserThunderRequestIdGenerator();
            app.UserThunderSecurityHeaders();

            app.UseMiddleware<ThunderAuthenticationMiddleware>();
            app.UseRouting();
            app.UseThunderCors();

            /* Remove UnitOfWork middleware app.UseThunderUnitOfWork(); */

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseThunderSwagger(ApiName, "v1");
        }
    }
}
