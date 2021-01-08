using System;
using cx.datahub.scheduling.jobs.shared;
using Hangfire;
using Hangfire.SqlServer;
using IdentityModel.Client;
using IdentityServer4.AccessTokenValidation;
using Microservice.Badge.Infrastructure.Extensions;
using Microservice.Badge.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Thunder.Platform.AspNetCore;
using Thunder.Platform.AspNetCore.Cors;
using Thunder.Platform.Core.Helpers;
using Thunder.Platform.Core.Timing;
using Thunder.Service.Authentication;

namespace Microservice.Badge
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string ApiName { get; } = "OPAL2.0 - Badge API";

        public string HangFireQueueName { get; } = "badge_api";

        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddMvc(options =>
                {
                    options.Filters.Add<AuthorizationFilter>();
                })
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
                });

            services.Configure<MvcOptions>(mvcOptions =>
            {
                mvcOptions.AddThunderMvcOptions();
            })
                .AddOptions<SystemTimeZoneOptions>().Bind(_configuration.GetSection(nameof(SystemTimeZoneOptions)))
                .Services.AddOptions<OpalClientUrlOption>().Bind(_configuration)
                .Services.AddOptions<HangfireScheduleOptions>().Bind(_configuration.GetSection(nameof(HangfireScheduleOptions)));
        }

        public void Configure(
            IApplicationBuilder app,
            IOptions<SystemTimeZoneOptions> systemTimeZoneOptions,
            IOptions<HangfireScheduleOptions> hangfireScheduleOptions)
        {
            app.UseThunderExceptionHandler();
            app.UserThunderRequestIdGenerator();
            app.UserThunderSecurityHeaders();

            app.UseMiddleware<ThunderAuthenticationMiddleware>();
            app.UseRouting();
            app.UseThunderCors();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseThunderSwagger(ApiName, "v1");

            var systemTimeZone = DateTimeHelper.GetSystemTimeZone(systemTimeZoneOptions.Value.Windows, systemTimeZoneOptions.Value.Linux);

            Clock.SetTimeZoneInfo(systemTimeZone);
            RegisterHangfireJobs(systemTimeZone, hangfireScheduleOptions.Value);
            app.UseSeedData();
        }

        private void RegisterHangfireJobs(
            TimeZoneInfo systemTimeZone,
            HangfireScheduleOptions hangfireScheduleOptions)
        {
            RecurringJob.AddOrUpdate<ISummarizeDailyStatisticsForBadging>(
                t => t.ExecuteTask(null),
                hangfireScheduleOptions.DailyStatistics,
                timeZone: systemTimeZone,
                queue: HangFireQueueName);
            RecurringJob.AddOrUpdate<ISummarizeMonthlyStatisticsForBadging>(
                t => t.ExecuteTask(null),
                hangfireScheduleOptions.MonthlyStatistics,
                timeZone: systemTimeZone,
                queue: HangFireQueueName);
        }
    }
}
