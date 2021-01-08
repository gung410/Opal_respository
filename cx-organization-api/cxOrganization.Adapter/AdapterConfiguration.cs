using System;
using cxOrganization.Adapter.Assessment.Data;
using cxOrganization.Adapter.Assessment.Data.Repositories;
using cxOrganization.Adapter.JobChannel;
using cxOrganization.Adapter.JobMatch;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace cxOrganization.Adapter
{
    public static class AdapterConfiguration
    {
        public static void UseAdapterConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<AssessmentContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));
            services.AddDbContext<AssessmentConfigContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            RegisterRepositories(services);

            var jobChannelAPIBaseUrl = configuration["AppSettings:JobChannelAPIBaseUrl"];
            var jobChannelAPISecretKey = configuration["AppSettings:JobChannelAPISecretKey"];
            if (string.IsNullOrEmpty(jobChannelAPIBaseUrl) && string.IsNullOrEmpty(jobChannelAPISecretKey))
                services.AddScoped<IJobChannelAdapter, UnusedJobChannelAdapter>();
            else
                services.AddScoped<IJobChannelAdapter, JobChannelAdapter>(serviceProvider => new JobChannelAdapter(jobChannelAPIBaseUrl, jobChannelAPISecretKey));

            services.AddScoped<IJobMatchAdapter, JobMatchAdapter>(serviceProvider => new JobMatchAdapter(configuration["AppSettings:JobChannelAPIBaseUrl"],
                                                                                                         configuration["AppSettings:JobChannelAPIBaseUrl"],
                                                                                                         serviceProvider.GetService<ILoggerFactory>()));
        }

        private static void RegisterRepositories(IServiceCollection services)
        {
            services.AddScoped<IActivityRepository, ActivityRepository>();
            services.AddScoped<IActivityStatusTypeRepository, ActivityStatusTypeRepository>();
            services.AddScoped<IAlternativeRepository, AlternativeRepository>();
            services.AddScoped<IAnswerRepository, AnswerRepository>();

            services.AddScoped<ILanguageRepository, LanguageRepository>();
            services.AddScoped<ILevelGroupRepository, LevelGroupRepository>();
            services.AddScoped<ILevelLimitRepository, LevelLimitRepository>();
            services.AddScoped<IResultRepository, ResultRepository>();
            services.AddScoped<IStatusTypeRepository, StatusTypeRepository>();

            services.AddScoped<ISurveyRepository, SurveyRepository>();
        }
    }
}
