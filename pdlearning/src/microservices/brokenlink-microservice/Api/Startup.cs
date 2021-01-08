using System;
using System.Runtime.InteropServices;
using Conexus.Opal.BrokenLinkChecker;
using cx.datahub.scheduling.jobs.shared;
using FluentValidation.AspNetCore;
using Hangfire;
using Hangfire.SqlServer;
using IdentityModel.Client;
using IdentityServer4.AccessTokenValidation;
using Microservice.BrokenLink.Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Thunder.Platform.AspNetCore;
using Thunder.Platform.AspNetCore.Cors;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;
using Thunder.Platform.Core.Helpers;
using Thunder.Platform.EntityFrameworkCore;
using Thunder.Service.Authentication;
using Thunder.Service.HangfireOption;

namespace Microservice.BrokenLink
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string ApiName { get; } = "OPAL2.0 - BrokenLink API";

        public string HangFireQueueName { get; } = "digital_content_api";

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
                        Authority = internalUrl, ValidateIssuerName = false
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
                .AddHangfire(configuration => configuration
                    .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                    .UseSimpleAssemblyNameTypeSerializer()
                    .UseRecommendedSerializerSettings()
                    .UseSqlServerStorage(
                        _configuration.GetConnectionString("HangfireDb"),
                        new SqlServerStorageOptions
                        {
                            CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                            SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                            QueuePollInterval = TimeSpan.FromSeconds(15),
                            UseRecommendedIsolationLevel = true,
                            UsePageLocksOnDequeue = true,
                            DisableGlobalLocks = true
                        }))
                .AddHangfireServer(options =>
                {
                    options.ServerName = $"{Environment.MachineName}.{Guid.NewGuid()}";
                    options.Queues = new[] { HangFireQueueName };
                })
                .AddOptions<DomainWhitelist>()
                .Bind(_configuration.GetSection(nameof(DomainWhitelist)));

            services.Configure<MvcOptions>(mvcOptions =>
            {
                mvcOptions.AddThunderMvcOptions();
            });
        }

        public void Configure(
            IApplicationBuilder app,
            IDbContextResolver dbContextResolver,
            IConnectionStringResolver connectionStringResolver)
        {
            dbContextResolver.InitDatabase<BrokenLinkDbContext>(
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

            RegisterHangfireJobs();
        }

        private void RegisterHangfireJobs()
        {
            var scanScheduleOption = _configuration.GetSection("BrokenLinkScheduleOption").Get<HangfireScheduleOption>();

            TimeZoneInfo systemTimeZone = DateTimeHelper.GetSystemTimeZone(scanScheduleOption.WindowTimezone, scanScheduleOption.LinuxTimezone);

            RecurringJob.AddOrUpdate<IBrokenLinkContentScanner>(
                t => t.ExecuteTask(null),
                scanScheduleOption.ScanSchedule,
                timeZone: systemTimeZone,
                queue: HangFireQueueName);
        }
    }
}
