using System;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using cx.datahub.scheduling.jobs.shared;
using Microservice.Content.Application.Commands;
using Microservice.Content.Domain.Entities;
using Microservice.Content.Domain.Enums;
using Microservice.Content.Versioning.Application.Dtos;
using Microservice.Content.Versioning.Entities;
using Microservice.Content.Versioning.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;
using Thunder.Platform.Core.Timing;
using Thunder.Platform.Cqrs;

namespace Microservice.Content.Application.HangfireJob
{
    public class DigitalContentDailyCheckingJob : BaseHangfireJob, IDigitalContentDailyCheckingJob
    {
        private const int BatchSize = 10;

        private readonly IThunderCqrs _thunderCqrs;
        private readonly IUnitOfWorkManager _uowManager;
        private readonly IVersionTrackingApplicationService _versionTrackingApplicationService;
        private readonly ILogger<DigitalContentDailyCheckingJob> _logger;
        private readonly IRepository<DigitalContent> _digitalContentRepository;

        public DigitalContentDailyCheckingJob(
            IServiceProvider serviceProvider,
            IUnitOfWorkManager unitOfWorkManager,
            IThunderCqrs thunderCqrs,
            IRepository<DigitalContent> digitalContentRepository,
            ILogger<DigitalContentDailyCheckingJob> logger,
            IVersionTrackingApplicationService versionTrackingApplicationService) : base(serviceProvider)
        {
            _uowManager = unitOfWorkManager;
            _thunderCqrs = thunderCqrs;
            _digitalContentRepository = digitalContentRepository;
            _logger = logger;
            _versionTrackingApplicationService = versionTrackingApplicationService;
        }

        protected override async Task InternalHandleAsync()
        {
            await AutoArchiveContentDailyChecking();
            await AutoPublishContentDailyChecking();
        }

        private async Task AutoArchiveContentDailyChecking()
        {
            bool continueToScan = true;

            using (var uow = _uowManager.Begin(TransactionScopeOption.Suppress))
            {
                while (continueToScan)
                {
                    var contentIdsToArchive = await _digitalContentRepository
                        .GetAll()
                        .Where(p => p.ArchiveDate.HasValue &&
                                    p.ArchiveDate.Value.Date == Clock.Now.Date &&
                                    p.Status != DigitalContentStatus.PendingForApproval &&
                                    p.Status != DigitalContentStatus.Published &&
                                    p.Status != DigitalContentStatus.ReadyToUse &&
                                    p.Status != DigitalContentStatus.Archived &&
                                    p.IsArchived == false)
                        .Take(BatchSize)
                        .Select(p => p.Id)
                        .ToListAsync();

                    if (contentIdsToArchive.Count == 0)
                    {
                        break;
                    }

                    foreach (var id in contentIdsToArchive)
                    {
                        var command = new ArchiveDigitalContentCommand
                        {
                            ContentId = id,
                            ArchiveBy = Guid.Empty
                        };

                        await _thunderCqrs.SendCommand(command);
                    }

                    continueToScan = contentIdsToArchive.Count == BatchSize;
                }

                await uow.CompleteAsync();
                _logger.LogInformation("[Archive Content By Archive Date Scanner] finished: {0}", Clock.Now.ToLongTimeString());
            }
        }

        private async Task AutoPublishContentDailyChecking()
        {
            bool continueToScan = true;

            using (var uow = _uowManager.Begin(TransactionScopeOption.Suppress))
            {
                while (continueToScan)
                {
                    var contents = await _digitalContentRepository
                        .GetAll()
                        .Where(p => p.AutoPublishDate.HasValue &&
                                    p.AutoPublishDate.Value.Date == Clock.Now.Date &&
                                    p.Status == DigitalContentStatus.Approved &&
                                    p.IsArchived == false)
                        .Take(BatchSize)
                        .ToListAsync();

                    if (contents.Count == 0)
                    {
                        break;
                    }

                    foreach (var existedContent in contents)
                    {
                        existedContent.Status = DigitalContentStatus.Published;
                        existedContent.ChangedDate = Clock.Now;

                        await _digitalContentRepository.UpdateAsync(existedContent);

                        await _versionTrackingApplicationService.CreateVersionTracking(new CreateVersionTrackingParameter
                        {
                            VersionSchemaType = VersionSchemaType.DigitalContent,
                            ObjectId = existedContent.Id,
                            UserId = Guid.Empty,
                            ActionComment = "Automatically Published",
                            CanRollback = false,
                            IncreaseMajorVersion = true,
                            IncreaseMinorVersion = false
                        });
                    }

                    continueToScan = contents.Count == BatchSize;
                }

                await uow.CompleteAsync();
                _logger.LogInformation("[Publish Content By Publish Date Scanner] finished: {0}", Clock.Now.ToLongTimeString());
            }
        }
    }
}
