using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microservice.WebinarVideoConverter.Application.Services.Abstractions;
using Microservice.WebinarVideoConverter.Domain.Entities;
using Microservice.WebinarVideoConverter.Domain.Enums;
using Microservice.WebinarVideoConverter.Infrastructure.Configurations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Microservice.WebinarVideoConverter.Application.BackgroundServices
{
    public class RecordingCollectorService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<RecordingCollectorService> _logger;
        private bool _applicationStarted = false;

        public RecordingCollectorService(
            IServiceProvider serviceProvider,
            IHostApplicationLifetime applicationLifetime,
            ILogger<RecordingCollectorService> logger)
        {
            applicationLifetime.ApplicationStarted.Register(() => _applicationStarted = true);
            applicationLifetime.ApplicationStopping.Register(() => _applicationStarted = false);
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("[{NameOfService}] started.", nameof(RecordingCollectorService));

            while (!stoppingToken.IsCancellationRequested)
            {
                if (!_applicationStarted)
                {
                    await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
                    continue;
                }

                try
                {
                    // Resolve dependencies.
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var convertingTrackingService = scope.ServiceProvider.GetService<IConvertingTrackingService>();
                        var playbackScannerService = scope.ServiceProvider.GetService<IPlaybackScannerService>();
                        var recordingConvertOptions = scope.ServiceProvider.GetService<IOptions<RecordingConvertOptions>>().Value;

                        // Scan folder for playback's metadata.
                        var metadataRecords = playbackScannerService.Collect(recordingConvertOptions.PlaybacksDir);

                        // Insert collected metadata.
                        var trackings = metadataRecords
                            .Select(metadataRecord => new ConvertingTracking
                            {
                                Id = Guid.NewGuid(),
                                InternalMeetingId = metadataRecord.InternalMeetingId,
                                MeetingId = metadataRecord.MeetingId,
                                Status = ConvertStatus.New
                            })
                            .ToList();

                        await convertingTrackingService.InsertIfNotExistsAsync(trackings);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to process. Message: {Message}, see details: {Details}", ex.Message, ex.StackTrace);
                }
                finally
                {
                    await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
                }
            }
        }
    }
}
