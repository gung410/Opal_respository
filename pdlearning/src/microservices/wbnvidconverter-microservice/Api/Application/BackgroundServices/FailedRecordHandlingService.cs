using System;
using System.Threading;
using System.Threading.Tasks;
using Microservice.WebinarVideoConverter.Application.Events;
using Microservice.WebinarVideoConverter.Application.RequestDtos;
using Microservice.WebinarVideoConverter.Application.Services.Abstractions;
using Microservice.WebinarVideoConverter.Domain.Entities;
using Microservice.WebinarVideoConverter.Domain.Enums;
using Microservice.WebinarVideoConverter.Infrastructure.Configurations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Thunder.Platform.Cqrs;

namespace Microservice.WebinarVideoConverter.Application.BackgroundServices
{
    public class FailedRecordHandlingService : BackgroundService
    {
        private const int DelaySeconds = 30;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<FailedRecordHandlingService> _logger;
        private bool _applicationStarted = false;

        public FailedRecordHandlingService(
            IServiceProvider serviceProvider,
            IHostApplicationLifetime applicationLifetime,
            ILogger<FailedRecordHandlingService> logger)
        {
            applicationLifetime.ApplicationStarted.Register(() => _applicationStarted = true);
            applicationLifetime.ApplicationStopping.Register(() => _applicationStarted = false);
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("[{NameOfService}] started.", nameof(FailedRecordHandlingService));

            while (!stoppingToken.IsCancellationRequested)
            {
                if (!_applicationStarted)
                {
                    await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
                    continue;
                }

                using (var scope = _serviceProvider.CreateScope())
                {
                    var convertingTrackingService = scope.ServiceProvider.GetService<IConvertingTrackingService>();
                    var thunderCqrs = scope.ServiceProvider.GetService<IThunderCqrs>();
                    var converterOptions = scope.ServiceProvider.GetService<IOptions<FailedRetryOptions>>().Value;

                    var failedRecords = await convertingTrackingService.GetFailedTrackings();
                    foreach (var failedRecord in failedRecords)
                    {
                        try
                        {
                            // RetryCount < MaxRetryCount, set to previous status.
                            if (failedRecord.RetryCount < converterOptions.MaximumRetryAttempts)
                            {
                                await ProcessRetry(failedRecord, convertingTrackingService);
                            }

                            // RetryCount >= MaxRetryCount, ignore retry and send events to notify failed records.
                            else
                            {
                                await ProcessIgnoreRetry(failedRecord, convertingTrackingService, thunderCqrs);
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(
                                ex,
                                "Failed to process retrying meeting {InternalMeetingId}. Message: {Message}. Details: {Details}",
                                failedRecord.InternalMeetingId,
                                ex.Message,
                                ex.StackTrace);
                        }
                    }
                }

                await Task.Delay(TimeSpan.FromSeconds(DelaySeconds), stoppingToken);
            }
        }

        private async Task ProcessRetry(ConvertingTracking failedRecord, IConvertingTrackingService convertingTrackingService)
        {
            _logger.LogInformation("Start to process retrying convert meeting id {MeetingId}.", failedRecord.InternalMeetingId);
            await convertingTrackingService.ProcessRetryRecord(failedRecord.Id);
        }

        private async Task ProcessIgnoreRetry(ConvertingTracking failedTracking, IConvertingTrackingService convertingTrackingService, IThunderCqrs thunderCqrs)
        {
            _logger.LogInformation("Start to ignore converting meeting {InternalMeetingId}", failedTracking.InternalMeetingId);
            await convertingTrackingService.ProcessIgnoreRetryRecord(failedTracking.Id);
            await thunderCqrs.SendEvent(new WebinarRecordChangeEvent(
                new WebinarRecordChangeRequest
                {
                    MeetingId = failedTracking.MeetingId,
                    RecordId = failedTracking.Id,
                    Status = ConvertStatus.IgnoreRetry,
                    InternalMeetingId = failedTracking.InternalMeetingId,
                }));
        }
    }
}
