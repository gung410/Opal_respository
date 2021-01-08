using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microservice.WebinarVideoConverter.Application.Events;
using Microservice.WebinarVideoConverter.Application.RequestDtos;
using Microservice.WebinarVideoConverter.Application.Services.Abstractions;
using Microservice.WebinarVideoConverter.Domain.Enums;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Thunder.Platform.Cqrs;

namespace Microservice.WebinarVideoConverter.Application.BackgroundServices
{
    public class RecordUploaderService : BackgroundService
    {
        private const int MaxUploadItems = 5;
        private const int DelayTime = 30;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<RecordUploaderService> _logger;
        private bool _applicationStarted = false;

        public RecordUploaderService(
            IServiceProvider serviceProvider,
            IHostApplicationLifetime applicationLifetime,
            ILogger<RecordUploaderService> logger)
        {
            applicationLifetime.ApplicationStarted.Register(() => _applicationStarted = true);
            applicationLifetime.ApplicationStopping.Register(() => _applicationStarted = false);
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("[{NameOfService}] started.", nameof(RecordUploaderService));

            while (!stoppingToken.IsCancellationRequested)
            {
                if (!_applicationStarted)
                {
                    await Task.Delay(TimeSpan.FromSeconds(DelayTime), stoppingToken);
                    continue;
                }

                try
                {
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var convertingTrackingService = scope.ServiceProvider.GetService<IConvertingTrackingService>();
                        var uploadService = scope.ServiceProvider.GetService<IUploaderService>();
                        var thunderCqrs = scope.ServiceProvider.GetService<IThunderCqrs>();

                        // Get record with status Converted and set status Uploading
                        var canUploadRecords = await convertingTrackingService.GetCanUploadRecord(MaxUploadItems);
                        if (canUploadRecords == null || !canUploadRecords.Any())
                        {
                            continue;
                        }

                        _logger.LogInformation("New upload records: {Count}", canUploadRecords.Count);

                        foreach (var canUploadRecord in canUploadRecords)
                        {
                            try
                            {
                                await convertingTrackingService.UpdateConvertTrackingStatusAsync(canUploadRecord.Id, ConvertStatus.Uploading);

                                var uploadResult = await uploadService.UploadMeetingRecordAsync(canUploadRecord.InternalMeetingId);

                                if (uploadResult.IsSuccess)
                                {
                                    // Set status as Done
                                    await convertingTrackingService.MarkTrackingAsCompleteAsync(canUploadRecord.Id, uploadResult.FilePath);

                                    // Inform meeting record is converted successfully
                                    await thunderCqrs.SendEvent(new WebinarRecordChangeEvent(
                                        new WebinarRecordChangeRequest
                                        {
                                            MeetingId = canUploadRecord.MeetingId,
                                            RecordId = canUploadRecord.Id,
                                            InternalMeetingId = canUploadRecord.InternalMeetingId,
                                            Status = ConvertStatus.Ready,
                                            VideoPath = uploadResult.FilePath,
                                            FileSize = uploadResult.FileSize,
                                        }));
                                }
                                else
                                {
                                    // Cannot find record to upload
                                    await convertingTrackingService.UpdateFailedConvertTrackingStatusAsync(canUploadRecord.Id, FailStep.Uploading);
                                    _logger.LogError("Failed to process. Cannot find record to upload : {InternalMeetingId}", canUploadRecord.InternalMeetingId);
                                }
                            }
                            catch (Exception ex)
                            {
                                // Set status as Fail
                                await convertingTrackingService.UpdateFailedConvertTrackingStatusAsync(canUploadRecord.Id, FailStep.Uploading);
                                _logger.LogError(ex, "Failed to process. Message: {Message}, see details: {StackTrace}", ex.Message, ex.StackTrace);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
                finally
                {
                    await Task.Delay(TimeSpan.FromSeconds(DelayTime), stoppingToken);
                }
            }
        }
    }
}
