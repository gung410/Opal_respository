using System;
using System.Linq;
using cx.datahub.scheduling.jobs.shared;
using cxOrganization.Domain.Settings;
using cxOrganization.WebServiceAPI.Hangfire;
using Hangfire;
using Hangfire.Server;
using Hangfire.SqlServer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace cxOrganization.WebServiceAPI.Background
{
    public static class HangfireExtension
    {
        public static IServiceCollection UseRecurringJobs(this IServiceCollection services, IConfiguration configuration, ILogger logger)
        {
            var recurringJobSettings = configuration.GetSection("RecurringJobSettings").Get<RecurringJobSettings>();

            services.AddRecurringJobSetting(recurringJobSettings)
                .RegisterHangfire(configuration, recurringJobSettings)
                .RegisterJobServices()
                .AddRecurringJobs(recurringJobSettings, logger);


            return services;
        }

        private static IServiceCollection AddRecurringJobSetting(this IServiceCollection services,  RecurringJobSettings recurringJobSettings)
        {
            services.Configure<RecurringJobSettings>(a =>
            {
                foreach (var recurringJobSetting in recurringJobSettings)
                {
                    a.Add(recurringJobSetting.Key, recurringJobSetting.Value);
                }
            });
            return services;
        }

        public static IServiceCollection AddChangeUserStatusSetting(this IServiceCollection services, ChangeUserStatusSettings changeUserStatusSettings)
        {
            services.Configure<ChangeUserStatusSettings>(a =>
            {
                foreach (var changeUserStatusSetting in changeUserStatusSettings)
                {
                    a.Add(changeUserStatusSetting.Key, changeUserStatusSetting.Value);
                }
            });
            return services;
        }

        private static IServiceCollection RegisterJobServices(this IServiceCollection services)
        {
            services.AddScoped<ISendWelcomeEmailJob, SendWelcomeEmailJob>();
            services.AddScoped<ISuspendAndDeactivateUserJob, DeactivateUserJob>();
            services.AddScoped<ISendBroadcastMessageJob, SendBroadcastMessageJob>();
            services.AddScoped<IArchiveUserJob, ArchiveUserJob>();
            services.AddScoped<IOrganizationSuspendUserJob, SuspendUserJob>();
            return services;
        }

        private static IServiceCollection RegisterHangfire(this IServiceCollection services, IConfiguration configuration, RecurringJobSettings recurringJobSettings)
        {
            
            services.AddHangfire(option => option.SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings());
            var connectionString = configuration.GetConnectionString("Hangfire");
            if (!string.IsNullOrEmpty(connectionString))
            {
                JobStorage.Current = new SqlServerStorage(connectionString);
                var queues = recurringJobSettings.Values.Where(v => v.Enable).Select(v => v.Queue).Distinct().ToArray();
                services.AddHangfireServer(options => { options.Queues = queues;});
            }

            return services;
        }

        private static IServiceCollection AddRecurringJobs(this IServiceCollection services, RecurringJobSettings recurringJobSettings, ILogger logger)
        {
            try
            {
                AddRecurringJob<ISendWelcomeEmailJob>(recurringJobSettings, logger, SendWelcomeEmailJob.JobId);
                AddRecurringJob<IOrganizationSuspendUserJob>(recurringJobSettings, logger, SuspendUserJob.JobId);
                AddRecurringJob<ISuspendAndDeactivateUserJob>(recurringJobSettings, logger, DeactivateUserJob.JobId);
                AddRecurringJob<ISendBroadcastMessageJob>(recurringJobSettings, logger, SendBroadcastMessageJob.JobId);
                AddRecurringJob<IArchiveUserJob>(recurringJobSettings, logger, ArchiveUserJob.JobId);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Cannot add RecurringJob to Hangfire database");
            }

            return services;
        }

        private static void AddRecurringJob<T>(RecurringJobSettings recurringJobSettings, ILogger logger, string jobId)
            where T : IBackgroundJob
        {
            if (recurringJobSettings.TryGetValue(jobId, out var jobSetting) && jobSetting.Enable)
            {
                RecurringJob.AddOrUpdate<T>(jobId, job => job.Execute(null),
                    jobSetting.CronExpression, queue: jobSetting.Queue);
                logger.LogWarning($"Successfully registering recurring job {jobId} with CronExpression {jobSetting.CronExpression}.");
            }
            else
            {
                logger.LogWarning($"Successfully unregistering recurring job {jobId}.");
            }
        }
    }
}
