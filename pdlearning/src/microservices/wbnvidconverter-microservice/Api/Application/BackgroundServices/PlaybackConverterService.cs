using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microservice.WebinarVideoConverter.Application.Services.Abstractions;
using Microservice.WebinarVideoConverter.Configuration;
using Microservice.WebinarVideoConverter.Domain.Enums;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Microservice.WebinarVideoConverter.Application.BackgroundServices
{
    public class PlaybackConverterService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<PlaybackConverterService> _logger;
        private bool _applicationStarted = false;

        public PlaybackConverterService(
            IServiceProvider serviceProvider,
            IHostApplicationLifetime applicationLifetime,
            ILogger<PlaybackConverterService> logger)
        {
            applicationLifetime.ApplicationStarted.Register(() => _applicationStarted = true);
            applicationLifetime.ApplicationStopping.Register(() => _applicationStarted = false);
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("[{NameOfService}] started.", nameof(PlaybackConverterService));

            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
                if (!_applicationStarted)
                {
                    continue;
                }

                using (var scope = _serviceProvider.CreateScope())
                {
                    var convertingTrackingService = scope.ServiceProvider.GetService<IConvertingTrackingService>();
                    var awsFargateTaskRunner = scope.ServiceProvider.GetService<IAwsFargateTaskRunnerService>();
                    var playbackOptions = scope.ServiceProvider.GetService<IOptions<PlaybackOptions>>().Value;

                    var newRecords = await convertingTrackingService.GetConvertingTrackingsByStatus(ConvertStatus.New);
                    _logger.LogInformation("[Playback converter Job] Start to convert for {NewRecordsCount} new records", newRecords.Count);

                    foreach (var newRecord in newRecords)
                    {
                        try
                        {
                            await convertingTrackingService.UpdateConvertTrackingStatusAsync(newRecord.Id, ConvertStatus.Converting);
                            var taskResponse = await awsFargateTaskRunner.RunConvertPlaybackTaskAsync(playbackOptions.PlaybackUrl + newRecord.InternalMeetingId);
                            _logger.LogInformation("Started converter task response {Response}", JsonSerializer.Serialize(taskResponse));
                        }
                        catch (Exception exception)
                        {
                            _logger.LogError(
                                exception,
                                "Failed to convert {ConvertTrackingId}. Message: {Message}. Details: {Details}",
                                newRecord.Id,
                                exception.Message,
                                exception.StackTrace);
                            await convertingTrackingService.UpdateFailedConvertTrackingStatusAsync(newRecord.Id, FailStep.Converting);
                        }
                    }
                }
            }
        }
    }
}
