using System;
using System.Threading.Tasks;
using cx.datahub.scheduling.jobs.shared;
using Hangfire.Server;

namespace Microservice.Form.Application.HangfireJob
{
    public abstract class BaseHangfireJob : IBackgroundJob
    {
        protected BaseHangfireJob(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
        }

        protected IServiceProvider ServiceProvider { get; }

        public void Execute(PerformContext performContext, params object[] inputs)
        {
            InternalHandle();
        }

        public Task ExecuteTask(PerformContext performContext, params object[] inputs)
        {
            return InternalHandleAsync();
        }

        protected virtual Task InternalHandleAsync()
        {
            return Task.CompletedTask;
        }

        protected virtual void InternalHandle()
        {
            // Do nothing.
        }
    }
}
