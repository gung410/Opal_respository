using System;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using cx.datahub.scheduling.jobs.shared;
using Microservice.Form.Application.Events;
using Microservice.Form.Application.Models;
using Microservice.Form.Application.SharedQueries;
using Microsoft.Extensions.Logging;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;
using Thunder.Platform.Core.Timing;
using Thunder.Platform.Cqrs;

namespace Microservice.Form.Application.HangfireJob
{
    public class FormWeeklyCheckingJob : BaseHangfireJob, IFormWeeklyCheckingJob
    {
        private const int BatchSize = 10;

        private readonly GetFormsSharedQuery _formsSharedQuery;
        private readonly IUnitOfWorkManager _uowManager;
        private readonly IThunderCqrs _thunderCqrs;
        private readonly ILogger<FormDailyCheckingJob> _logger;

        public FormWeeklyCheckingJob(
            GetFormsSharedQuery formsSharedQuery,
            ILogger<FormDailyCheckingJob> logger,
            IServiceProvider serviceProvider,
            IUnitOfWorkManager unitOfWorkManager,
            IThunderCqrs thunderCqrs) : base(serviceProvider)
        {
            _uowManager = unitOfWorkManager;
            _thunderCqrs = thunderCqrs;
            _logger = logger;
            _formsSharedQuery = formsSharedQuery;
        }

        protected override async Task InternalHandleAsync()
        {
            await ArchiveFormWeeklyChecking();
        }

        private async Task ArchiveFormWeeklyChecking()
        {
            var oneYearBefore = Clock.Now.AddYears(-1);

            using (var uow = _uowManager.Begin(TransactionScopeOption.Suppress))
            {
                var formIds = await _formsSharedQuery.CanArchiveBecauseNoUsedForLongTime(oneYearBefore);

                if (formIds.Count == 0)
                {
                    return;
                }

                for (int i = 0; i < formIds.Count; i += BatchSize)
                {
                    var listIds = formIds.Skip(i).Take(BatchSize).ToList();

                    await _thunderCqrs.SendEvent(new CheckHasReferenceToResourceEvent(
                         new CheckHasReferenceToResourceModel
                         {
                             ObjectIds = listIds,
                             ContentType = CourseReferencedContentType.Form
                         }));
                }

                await uow.CompleteAsync();
                _logger.LogInformation("[Archive Form Not Used For Long Time Scanner] Finished: {0}", Clock.Now.ToLongTimeString());
            }
        }
    }
}
