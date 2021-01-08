using System;
using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using cx.datahub.scheduling.jobs.shared;
using FluentValidation.AspNetCore;
using Hangfire;
using Hangfire.SqlServer;
using IdentityModel.Client;
using IdentityServer4.AccessTokenValidation;
using Microservice.Course.Application.HangfireJob.RecurringJob;
using Microservice.Course.Common.Extensions;
using Microservice.Course.Infrastructure;
using Microservice.Course.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Thunder.Platform.AspNetCore;
using Thunder.Platform.AspNetCore.Cors;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;
using Thunder.Platform.Core.Timing;
using Thunder.Platform.EntityFrameworkCore;
using Thunder.Service.Authentication;

namespace Microservice.Course
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string ApiName { get; } = "OPAL2.0 - Course API";

        public string HangFireQueueName { get; } = "course_api";

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

            services
                .Configure<MvcOptions>(mvcOptions =>
                {
                    mvcOptions.AddThunderMvcOptions();
                })
                .AddOptions<SystemTimeZoneOption>().Bind(_configuration.GetSection(SystemTimeZoneOption.SystemTimeZone))
                .Services.AddOptions<OpalClientUrlOption>().Bind(_configuration)
                .Services.AddOptions<OpalSettingsOption>().Bind(_configuration.GetSection(OpalSettingsOption.OpalSettings))
                .Services.AddOptions<CompleteLearningProcessOption>().Bind(_configuration.GetSection(CompleteLearningProcessOption.ConfigurationSectionKey))
                .Services.AddOptions<AmazonS3Options>().Bind(_configuration.GetSection(nameof(AmazonS3Options)));

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
            IConnectionStringResolver connectionStringResolver,
            IOptions<SystemTimeZoneOption> systemTimeZoneOption)
        {
            dbContextResolver.InitDatabase<CourseDbContext>(
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

            var systemTimeZone = TimeHelper.GetSystemTimeZoneInfo(systemTimeZoneOption.Value);

            Clock.SetTimeZoneInfo(systemTimeZone);
            RegisterHangfireJobs(systemTimeZone);
        }

        private void RegisterHangfireJobs(TimeZoneInfo systemTimeZone)
        {
            RecurringJob.AddOrUpdate<IAttendanceCheckingJob>(t => t.ExecuteTask(null), Cron.Daily, systemTimeZone, HangFireQueueName);
            RecurringJob.AddOrUpdate<IOfferExpirationCheckingJob>(t => t.ExecuteTask(null), Cron.Daily, systemTimeZone, HangFireQueueName);
            RecurringJob.AddOrUpdate<ICourseDailyCheckingJob>(t => t.ExecuteTask(null), Cron.Daily, systemTimeZone, HangFireQueueName);
            RecurringJob.AddOrUpdate<IContentDailyCheckingJob>(t => t.ExecuteTask(null), Cron.Daily, systemTimeZone, HangFireQueueName);
            RecurringJob.AddOrUpdate<IRemindTakingAttendanceJob>(t => t.ExecuteTask(null), "*/15 * * * *", systemTimeZone, HangFireQueueName);
            RecurringJob.AddOrUpdate<IAssignmentDailyCheckingJob>(t => t.ExecuteTask(null), Cron.Daily, systemTimeZone, HangFireQueueName);
            RecurringJob.AddOrUpdate<IAnnouncementCheckingJob>(t => t.ExecuteTask(null), $"*/{AnnouncementCheckingJob.CheckingIntervalMinutes} * * * *", systemTimeZone, HangFireQueueName);
            RecurringJob.AddOrUpdate<ICoursePlanningCycleCheckingJob>(t => t.ExecuteTask(null), Cron.Daily, systemTimeZone, HangFireQueueName);
            RecurringJob.AddOrUpdate<IAssignmentExtendedCheckingJob>(t => t.ExecuteTask(null), "0 10 * * *", systemTimeZone, HangFireQueueName);
            RecurringJob.AddOrUpdate<IClassrunRegistrationDailyCheckingJob>(t => t.ExecuteTask(null), Cron.Daily, systemTimeZone, HangFireQueueName);
        }
    }
}
