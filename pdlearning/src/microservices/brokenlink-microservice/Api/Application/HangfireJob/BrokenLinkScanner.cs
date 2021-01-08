using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Conexus.Opal.BrokenLinkChecker;
using cx.datahub.scheduling.jobs.shared;
using Microservice.BrokenLink.Application.Services;
using Microservice.BrokenLink.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;
using Thunder.Platform.Core.Timing;

namespace Microservice.BrokenLink.Application.HangfireJob
{
    public class BrokenLinkScanner : BaseHangfireJob, IBrokenLinkContentScanner
    {
        private const int BatchSize = 10;

        private readonly ILogger<BrokenLinkScanner> _logger;
        private readonly DomainWhitelist _domainWhitelist;

        public BrokenLinkScanner(
            IServiceProvider serviceProvider,
            ILogger<BrokenLinkScanner> logger,
            IOptions<DomainWhitelist> domainWhitelistOptions) : base(serviceProvider)
        {
            _logger = logger;
            _domainWhitelist = domainWhitelistOptions.Value;
        }

        protected override async Task InternalHandleAsync()
        {
            bool continueScan = true;

            while (continueScan)
            {
                // Because each loop will need to complete the transaction for saving data into the database.
                // By default, if we inject IUnitOfWorkManager from the constructor and open a new transaction each loop
                // But by this way, we'll be faced with the issue that the DbContext and related objects can't be destroyed
                // So in order to avoid memory leak due to increased objects, the scope will be created inside the loop.
                using (var serviceScope = ServiceProvider.CreateScope())
                {
                    var uowManager = serviceScope.ServiceProvider.GetService<IUnitOfWorkManager>();
                    var brokenLinkNotifier = serviceScope.ServiceProvider.GetService<IBrokenLinkNotifier>();
                    var brokenLinkChecker = serviceScope.ServiceProvider.GetService<IBrokenLinkChecker>();
                    var extractedUrlRepository = serviceScope.ServiceProvider.GetService<IRepository<ExtractedUrl>>();
                    var brokenLinkReportRepository = serviceScope
                        .ServiceProvider
                        .GetService<IRepository<BrokenLinkReport>>();

                    _logger.LogInformation("BrokenLinkScanner begins to scan url.");
                    var notScannedUrls = await TakeNotScannedUrl(uowManager, extractedUrlRepository);

                    if (notScannedUrls == null || !notScannedUrls.Any())
                    {
                        _logger.LogInformation("There is no url to scan.");
                        break;
                    }

                    var sw = Stopwatch.StartNew();

                    _logger.LogInformation("BrokenLinkScanner scanning {UrlCount} url(s)", notScannedUrls.Count);

                    foreach (var urlInfo in notScannedUrls)
                    {
                        await uowManager.StartNewTransactionAsync(async () =>
                        {
                            var urlStatus = await brokenLinkChecker.CheckUrlAsync(urlInfo.Url, _domainWhitelist.Domains);
                            var extractedUrl = await extractedUrlRepository.GetAsync(urlInfo.Id);
                            extractedUrl.ScannedAt = Clock.Now;

                            extractedUrl.Status = urlStatus.IsValid ? ScanUrlStatus.Valid : ScanUrlStatus.Invalid;

                            await extractedUrlRepository.UpdateAsync(extractedUrl);

                            if (!urlStatus.IsValid)
                            {
                                var brokenLinkReport = new BrokenLinkReport
                                {
                                    Id = Guid.NewGuid(),
                                    ObjectId = urlInfo.ObjectId,
                                    OriginalObjectId = urlInfo.OriginalObjectId,
                                    ParentId = urlInfo.ParentId,
                                    ReportBy = Guid.Empty,
                                    Url = urlInfo.Url,
                                    Module = urlInfo.Module,
                                    ContentType = extractedUrl.ContentType,
                                    Description = urlStatus.InvalidReason,
                                    ObjectDetailUrl = extractedUrl.ObjectDetailUrl,
                                    ObjectOwnerId = extractedUrl.ObjectOwnerId,
                                    ObjectOwnerName = extractedUrl.ObjectOwnerName,
                                    ObjectTitle = extractedUrl.ObjectTitle,
                                    IsSystemReport = true,
                                };

                                await brokenLinkReportRepository.InsertAsync(brokenLinkReport);

                                await brokenLinkNotifier.NotifyBrokenLinkFound(new Models.BrokenLinkReportModel(brokenLinkReport));
                            }
                        });
                    }

                    _logger.LogInformation("BrokenLinkScanner scanned {UrlCount} url(s) took {TotalTime} milliseconds", notScannedUrls.Count, sw.ElapsedMilliseconds);
                    continueScan = notScannedUrls.Count == BatchSize;
                }
            }
        }

        private async Task<List<ExtractedUrl>> TakeNotScannedUrl(IUnitOfWorkManager uowManager, IRepository<ExtractedUrl> extractedUrlRepository)
        {
            var result = new TaskCompletionSource<List<ExtractedUrl>>();

            await uowManager.StartNewTransactionAsync(async () =>
            {
                try
                {
                    // Take top <BatchSize> urls that was not scanned or scanned over last 10 days.
                    var urlsToScan = await extractedUrlRepository
                        .GetAll()
                        .Where(p => (p.Status == ScanUrlStatus.None
                                    || !p.ScannedAt.HasValue
                                    || p.ScannedAt <= Clock.Now.AddDays(-10))
                                    && (p.Status != ScanUrlStatus.Checking))
                        .Take(BatchSize)
                        .ToListAsync();

                    foreach (var extractedUrl in urlsToScan)
                    {
                        extractedUrl.Status = ScanUrlStatus.Checking;
                    }

                    await extractedUrlRepository.UpdateManyAsync(urlsToScan);

                    result.SetResult(urlsToScan);
                }
                catch (Exception exception)
                {
                    // Ignore the exception.
                    _logger.LogWarning(exception, "There is an exception when taking url to scan.");

                    // Set to the empty list for safety purpose.
                    result.SetResult(new List<ExtractedUrl>());
                }
            });

            return await result.Task;
        }
    }
}
