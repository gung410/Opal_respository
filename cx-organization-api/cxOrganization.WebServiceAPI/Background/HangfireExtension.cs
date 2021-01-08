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
            services.AddScoped<ISuspendUserJob, SuspendUserJob>();
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
        
                if (recurringJobSettings.TryGetValue(SendWelcomeEmailJob.JobId, out var sendWelcomeEmailJobSetting) && sendWelcomeEmailJobSetting.Enable)
                {
                    RecurringJob.AddOrUpdate<ISendWelcomeEmailJob>(SendWelcomeEmailJob.JobId, job => job.Execute(null),
                        sendWelcomeEmailJobSetting.CronExpression, queue: sendWelcomeEmailJobSetting.Queue);
                    logger.LogInformation($"Add RecurringJob with id {SendWelcomeEmailJob.JobId}");
                }
                else
                {
                    logger.LogWarning($"RecurringJob with id {SendWelcomeEmailJob.JobId} has NOT been added because missing setting or disabled.");
                }

                if (recurringJobSettings.TryGetValue(SuspendUserJob.JobId, out var suspendUserJobSetting) && suspendUserJobSetting.Enable)
                {
                    RecurringJob.AddOrUpdate<ISuspendUserJob>(SuspendUserJob.JobId, job => job.Execute(null),
                        suspendUserJobSetting.CronExpression, queue: suspendUserJobSetting.Queue);
                    logger.LogInformation($"Add RecurringJob with id {SuspendUserJob.JobId}");
                }
                else
                {
                    logger.LogWarning($"RecurringJob with id {SuspendUserJob.JobId} has NOT been added because missing setting or disabled.");
                }

                if (recurringJobSettings.TryGetValue(DeactivateUserJob.JobId, out var deactivateUserJobSetting) && deactivateUserJobSetting.Enable)
                {
                    RecurringJob.AddOrUpdate<ISuspendAndDeactivateUserJob>(DeactivateUserJob.JobId, job => job.Execute(null),
                        deactivateUserJobSetting.CronExpression, queue: deactivateUserJobSetting.Queue);
                    logger.LogInformation($"Add RecurringJob with id {DeactivateUserJob.JobId}");
                }
                else
                {
                    logger.LogWarning($"RecurringJob with id {DeactivateUserJob.JobId} has NOT been added because missing setting or disabled.");
                }

                if (recurringJobSettings.TryGetValue(SendBroadcastMessageJob.JobId, out var sendBroadcastMessageJobSetting) && sendBroadcastMessageJobSetting.Enable)
                {
                    RecurringJob.AddOrUpdate<ISendBroadcastMessageJob>(SendBroadcastMessageJob.JobId, job => job.Execute(null),
                        sendBroadcastMessageJobSetting.CronExpression, queue: sendBroadcastMessageJobSetting.Queue);
                    logger.LogInformation($"Add RecurringJob with id {SendBroadcastMessageJob.JobId}");
                }
                else
                {
                    logger.LogWarning($"RecurringJob with id {SendBroadcastMessageJob.JobId} has NOT been added because missing setting or disabled.");
                }

                if (recurringJobSettings.TryGetValue(ArchiveUserJob.JobId, out var archiveUserJobSetting) && archiveUserJobSetting.Enable)
                {
                    RecurringJob.AddOrUpdate<IArchiveUserJob>(ArchiveUserJob.JobId, job => job.Execute(null),
                        archiveUserJobSetting.CronExpression, queue: archiveUserJobSetting.Queue);
                    logger.LogInformation($"Add RecurringJob with id {ArchiveUserJob.JobId}");
                }
                else
                {
                    logger.LogWarning($"RecurringJob with id {ArchiveUserJob.JobId} has NOT been added because missing setting or disabled.");
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Cannot add RecurringJob to Hangfire database");
            }

            return services;
        }
       
    }
}
