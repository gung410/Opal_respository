using cx.datahub.scheduling.jobs.shared;
using cxOrganization.Domain.Services;
using Hangfire.Server;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace cxOrganization.WebServiceAPI.Background
{
    public class DeactivateUserJob : ISuspendAndDeactivateUserJob
    {
        public const string JobId = "DeactivateUserJob";
        private readonly ISuspendOrDeactiveUserBackgroundJob _suspendOrDeactiveUserBackgroundJob;
        private readonly ILogger<DeactivateUserJob> _logger;


        public DeactivateUserJob(
            ISuspendOrDeactiveUserBackgroundJob suspendOrDeactiveUserBackgroundJob,
            ILogger<DeactivateUserJob> logger)
        {
            _suspendOrDeactiveUserBackgroundJob = suspendOrDeactiveUserBackgroundJob;
            _logger = logger;
        }

        public void Execute(PerformContext performContext, params object[] inputs)
        {
            _logger.LogInformation($"DeactivateUserJob Job started - Id: {performContext.BackgroundJob.Id}, Name: {performContext.BackgroundJob.Job.Type.FullName}");
            _suspendOrDeactiveUserBackgroundJob.DeActiveUserStatus().Wait();
            _logger.LogInformation($"DeactivateUserJob Job finished - Id: {performContext.BackgroundJob.Id}, Name: {performContext.BackgroundJob.Job.Type.FullName}");
        }

        public Task ExecuteTask(PerformContext performContext, params object[] inputs)
        {
            return Task.CompletedTask;
        }
    }
}
