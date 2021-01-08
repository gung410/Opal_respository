using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using Microservice.WebinarVideoConverter.Application.Services.Abstractions;
using Microservice.WebinarVideoConverter.Domain.Entities;
using Microservice.WebinarVideoConverter.Domain.Enums;
using Microservice.WebinarVideoConverter.Infrastructure.Configurations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;

namespace Microservice.WebinarVideoConverter.Application.BackgroundServices
{
    public class RecordCleanerService : BackgroundService
    {
        private const int DelayMinute = 30;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<RecordCleanerService> _logger;
        private readonly RecordingConvertOptions _recordingConvertOptions;
        private bool _applicationStarted;

        public RecordCleanerService(
            IServiceProvider serviceProvider,
            IHostApplicationLifetime applicationLifetime,
            ILogger<RecordCleanerService> logger,
            IOptions<RecordingConvertOptions> recordingConvertOptions)
        {
            applicationLifetime.ApplicationStarted.Register(() => _applicationStarted = true);
            applicationLifetime.ApplicationStopping.Register(() => _applicationStarted = false);
            _serviceProvider = serviceProvider;
            _logger = logger;
            _recordingConvertOptions = recordingConvertOptions.Value;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("[{NameOfService}] started", nameof(RecordCleanerService));

            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(TimeSpan.FromMinutes(DelayMinute), stoppingToken);
                if (!_applicationStarted)
                {
                    continue;
                }

                using (var scope = _serviceProvider.CreateScope())
                {
                    var fileRecordConvertedService = scope.ServiceProvider.GetService<IRecordFileService>();
                    var unitOfWorkManager = scope.ServiceProvider.GetService<IUnitOfWorkManager>();
                    var convertTrackingRepo = scope.ServiceProvider.GetService<IRepository<ConvertingTracking>>();

                    var convertedMeetingIds = fileRecordConvertedService.GetConvertedFileNames();
                    if (!convertedMeetingIds.Any())
                    {
                        continue;
                    }

                    using (var uow = unitOfWorkManager.Begin(TransactionScopeOption.Suppress))
                    {
                        foreach (var internalMetingId in convertedMeetingIds)
                        {
                            var convertTracking = await convertTrackingRepo
                                .FirstOrDefaultAsync(p =>
                                    p.InternalMeetingId == internalMetingId);
                            if (convertTracking == null || convertTracking.Status != ConvertStatus.Ready)
                            {
                                continue;
                            }

                            try
                            {
                                var convertedFilePath = $"{_recordingConvertOptions.ConvertedVideoDir}/{internalMetingId}.mp4";

                                // Delete video file.
                                File.Delete(convertedFilePath);

                                // Delete playback folder.
                                var playbackDirectory = $"{_recordingConvertOptions.PlaybacksDir}/{internalMetingId}";
                                Directory.Delete(playbackDirectory, recursive: true);
                            }
                            catch (Exception exception)
                            {
                                _logger.LogWarning("Failed to clean record {InternalMeetingId}. Detail: {Error} ", internalMetingId, exception.Message);
                            }
                        }
                    }
                }
            }
        }
    }
}
