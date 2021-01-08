using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using cx.datahub.scheduling.jobs.shared;
using Hangfire.Server;
using Microservice.Badge.Infrastructure;
using MongoDB.Driver;

namespace Microservice.Badge.Application.HangfireJob
{
    public abstract class BaseHangfireJob : IBackgroundJob
    {
        private const int PageSize = 200;

        protected BaseHangfireJob(BadgeDbContext dbContext)
        {
            BadgeDbContext = dbContext;
        }

        protected BadgeDbContext BadgeDbContext { get; }

        protected AggregateOptions AggregateOptions => new()
        {
            AllowDiskUse = true,
            BatchSize = PageSize
        };

        public void Execute(PerformContext performContext, params object[] inputs)
        {
            InternalHandle();
        }

        public Task ExecuteTask(PerformContext performContext, params object[] inputs)
        {
            return InternalHandleAsync();
        }

        protected virtual Task InternalHandleAsync(CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        protected virtual void InternalHandle()
        {
            // Do nothing.
        }
    }
}
