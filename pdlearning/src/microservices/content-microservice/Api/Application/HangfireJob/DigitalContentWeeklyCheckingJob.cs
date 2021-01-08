using System;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using cx.datahub.scheduling.jobs.shared;
using Microservice.Content.Application.Events.CheckHasReferenceToResource;
using Microservice.Content.Application.Models;
using Microservice.Content.Domain.Entities;
using Microservice.Content.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;
using Thunder.Platform.Core.Timing;
using Thunder.Platform.Cqrs;

namespace Microservice.Content.Application.HangfireJob
{
    public class DigitalContentWeeklyCheckingJob : BaseHangfireJob, IDigitalContentWeeklyCheckingJob
    {
        private const int BatchSize = 25;

        private readonly IUnitOfWorkManager _uowManager;
        private readonly IThunderCqrs _thunderCqrs;
        private readonly IRepository<DigitalContent> _digitalContentRepository;
        private readonly ILogger<DigitalContentWeeklyCheckingJob> _logger;

        public DigitalContentWeeklyCheckingJob(
            IServiceProvider serviceProvider,
            IUnitOfWorkManager unitOfWorkManager,
            IThunderCqrs thunderCqrs,
            IRepository<DigitalContent> digitalContentRepository,
            ILogger<DigitalContentWeeklyCheckingJob> logger) : base(serviceProvider)
        {
            _uowManager = unitOfWorkManager;
            _thunderCqrs = thunderCqrs;
            _digitalContentRepository = digitalContentRepository;
            _logger = logger;
        }

        protected override async Task InternalHandleAsync()
        {
            await ArchiveContentNotUsedForLongTimeChecking();
        }

        private async Task ArchiveContentNotUsedForLongTimeChecking()
        {
            var oneYearBefore = Clock.Now.AddYears(-1);

            // Using only one transaction to avoid increased DbContext.
            using (var uow = _uowManager.Begin(TransactionScopeOption.Suppress))
            {
                var itemsNeedCheckHasReference = await _digitalContentRepository
                    .GetAll()
                    .Where(p => p.ChangedDate.Value.Date <= oneYearBefore.Date &&
                                p.Status != DigitalContentStatus.Archived &&
                                p.IsArchived == false)
                    .Select(p => p.Id)
                    .ToListAsync();

                if (itemsNeedCheckHasReference.Count == 0)
                {
                    return;
                }

                for (int i = 0; i < itemsNeedCheckHasReference.Count; i += BatchSize)
                {
                    var listIds = itemsNeedCheckHasReference.Skip(i).Take(BatchSize).ToList();

                    await _thunderCqrs.SendEvent(new CheckHasReferenceToResourceEvent(
                           new CheckHasReferenceToResourceModel
                           {
                               ObjectIds = listIds,
                               ContentType = CourseReferencedContentType.Content
                           }));
                }

                await uow.CompleteAsync();
            }

            _logger.LogInformation("[ArchiveContentNotUsedForLongTimeScanner] finished: {0}", Clock.Now.ToLongTimeString());
        }
    }
}
