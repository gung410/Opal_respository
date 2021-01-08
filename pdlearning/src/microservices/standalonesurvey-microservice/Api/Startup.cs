using System;
using cx.datahub.scheduling.jobs.shared;
using FluentValidation.AspNetCore;
using Hangfire;
using Hangfire.SqlServer;
using IdentityModel.Client;
using IdentityServer4.AccessTokenValidation;
using Microservice.StandaloneSurvey.Infrastructure;
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

namespace Microservice.StandaloneSurvey
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string ApiName { get; } = "OPAL2.0 - Form API";

        public string HangFireQueueName { get; } = "form_api";

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
                });

            services.AddHangfire(configuration => configuration
               .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
               .UseSimpleAssemblyNameTypeSerializer()
               .UseRecommendedSerializerSettings()
               .UseSqlServerStorage(_configuration.GetConnectionString("HangfireDb"), new SqlServerStorageOptions
               {
                   CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                   SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                   QueuePollInterval = TimeSpan.FromSeconds(15),
                   UseRecommendedIsolationLevel = true,
                   UsePageLocksOnDequeue = true,
                   DisableGlobalLocks = true
               }));

            services.AddHangfireServer(options =>
            {
                options.ServerName = string.Format("{0}.{1}", Environment.MachineName, Guid.NewGuid().ToString());
                options.Queues = new[] { HangFireQueueName };
            });
        }

        public void Configure(
            IApplicationBuilder app,
            IDbContextResolver dbContextResolver,
            IConnectionStringResolver connectionStringResolver)
        {
            dbContextResolver.InitDatabase<StandaloneSurveyDbContext>(
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
            var archiveByDateOption = _configuration.GetSection("ArchiveByDateScheduleOption").Get<HangfireScheduleOption>();
            var archiveByCheckRefOption = _configuration.GetSection("ArchiveByCheckReferenceScheduleOption").Get<HangfireScheduleOption>();

            TimeZoneInfo systemTimeZone = DateTimeHelper.GetSystemTimeZone(archiveByDateOption.WindowTimezone, archiveByDateOption.LinuxTimezone);

            RecurringJob.AddOrUpdate<IArchiveFormScanner>(
                t => t.ExecuteTask(null),
                archiveByDateOption.ScanSchedule,
                queue: HangFireQueueName,
                timeZone: systemTimeZone);

            RecurringJob.AddOrUpdate<ICheckReferenceForArchiveFormScanner>(
                t => t.ExecuteTask(null),
                archiveByCheckRefOption.ScanSchedule,
                queue: HangFireQueueName,
                timeZone: systemTimeZone);
        }
    }
}
