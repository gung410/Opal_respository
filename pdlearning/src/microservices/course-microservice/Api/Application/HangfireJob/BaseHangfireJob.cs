using System;
using System.Threading.Tasks;
using System.Transactions;
using cx.datahub.scheduling.jobs.shared;
using Hangfire.Server;
using Microsoft.Extensions.Logging;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;
using Thunder.Platform.Cqrs;

namespace Microservice.Course.Application.HangfireJob.RecurringJob
{
    public abstract class BaseHangfireJob : IBackgroundJob
    {
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly ILogger _logger;

        protected BaseHangfireJob(IThunderCqrs thunderCqrs, IUnitOfWorkManager unitOfWorkManager, ILoggerFactory loggerFactory)
        {
            ThunderCqrs = thunderCqrs;
            _unitOfWorkManager = unitOfWorkManager;
            _logger = loggerFactory.CreateLogger<BaseHangfireJob>();
        }

        protected IThunderCqrs ThunderCqrs { get; }

        public void Execute(PerformContext performContext, params object[] inputs)
        {
            using (var uow = _unitOfWorkManager.Begin(TransactionScopeOption.Suppress))
            {
                InternalHandle();
                uow.Complete();
            }
        }

        public async Task ExecuteTask(PerformContext performContext, params object[] inputs)
        {
            using (var uow = _unitOfWorkManager.Begin(TransactionScopeOption.Suppress))
            {
                await InternalHandleAsync();
                await uow.CompleteAsync();
            }
        }

        protected virtual Task InternalHandleAsync()
        {
            return Task.CompletedTask;
        }

        protected virtual void InternalHandle()
        {
            // Do nothing.
        }

        protected async Task ByPassTaskException(Func<Task> taskFn, string taskName)
        {
            try
            {
                await taskFn();
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Some thing went wrong in task {taskName} of job {GetType().Name}");
            }
        }
    }
}
