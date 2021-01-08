using cx.datahub.scheduling.jobs.shared;
using cxOrganization.Domain.Services;
using cxPlatform.Client.ConexusBase;
using Hangfire.Server;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace cxOrganization.WebServiceAPI.Hangfire
{
    public class SuspendUserJob : ISuspendUserJob
    {
        public const string JobId = "SuspendUserJob";
        private readonly ISuspendOrDeactiveUserBackgroundJob _suspendOrDeactiveUserBackgroundJob;
        private readonly ILogger<SuspendUserJob> _logger;


        public SuspendUserJob(
            ISuspendOrDeactiveUserBackgroundJob suspendOrDeactiveUserBackgroundJob,
            ILogger<SuspendUserJob> logger)
        {
            _suspendOrDeactiveUserBackgroundJob = suspendOrDeactiveUserBackgroundJob;
            _logger = logger;
        }

        public void Execute(PerformContext performContext, params object[] inputs)
        {
            _logger.LogInformation($"ISuspendUserJob Job started - Id: {performContext.BackgroundJob.Id}, Name: {performContext.BackgroundJob.Job.Type.FullName}");
            _suspendOrDeactiveUserBackgroundJob.SuspendUserStatus().Wait();
            _logger.LogInformation($"ISuspendUserJob Job finished - Id: {performContext.BackgroundJob.Id}, Name: {performContext.BackgroundJob.Job.Type.FullName}");
        }

        public Task ExecuteTask(PerformContext performContext, params object[] inputs)
        {
            return Task.CompletedTask;
        }
    }
}
