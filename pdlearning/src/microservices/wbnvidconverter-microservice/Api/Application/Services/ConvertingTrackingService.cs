using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microservice.WebinarVideoConverter.Application.Commands;
using Microservice.WebinarVideoConverter.Application.Models;
using Microservice.WebinarVideoConverter.Application.Queries;
using Microservice.WebinarVideoConverter.Application.Services.Abstractions;
using Microservice.WebinarVideoConverter.Domain.Entities;
using Microservice.WebinarVideoConverter.Domain.Enums;
using Microservice.WebinarVideoConverter.Infrastructure.Configurations;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Thunder.Platform.Cqrs;

namespace Microservice.WebinarVideoConverter.Application.Services
{
    public class ConvertingTrackingService : IConvertingTrackingService
    {
        private readonly IThunderCqrs _thunderCqrs;
        private readonly ILogger<ConvertingTrackingService> _logger;
        private readonly RecordingConvertOptions _recordingConvertOptions;

        public ConvertingTrackingService(
            IThunderCqrs thunderCqrs,
            ILogger<ConvertingTrackingService> logger,
            IOptions<RecordingConvertOptions> recordingConvertOptions)
        {
            _thunderCqrs = thunderCqrs;
            _recordingConvertOptions = recordingConvertOptions.Value;
            _logger = logger;
        }

        /// <inheritdoc/>
        public Task<List<ConvertingTrackingModel>> GetConvertingTrackingsByStatus(ConvertStatus status)
        {
            return _thunderCqrs.SendQuery(new GetConvertingTrackingsByStatusQuery
            {
                Status = status
            });
        }

        /// <inheritdoc/>
        public Task InsertIfNotExistsAsync(IEnumerable<ConvertingTracking> trackings)
        {
            return _thunderCqrs.SendCommand(new SaveConvertingTrackingsCommand
            {
                ConvertTrackings = trackings
            });
        }

        public Task<List<ConvertingTracking>> GetFailedTrackings()
        {
            return _thunderCqrs.SendQuery(new GetFailedTrackingsQuery());
        }

        public async Task ProcessRetryRecord(Guid convertingTrackingId)
        {
            await _thunderCqrs.SendCommand(new ProcessRetryTrackingCommand
            {
                ConvertingTrackingId = convertingTrackingId
            });
        }

        public async Task ProcessIgnoreRetryRecord(Guid convertingTrackingId)
        {
            await _thunderCqrs.SendCommand(new ProcessIgnoreRetryTrackingCommand
            {
                ConvertingTrackingId = convertingTrackingId
            });
        }

        public Task<List<ConvertingTrackingModel>> GetCanUploadRecord(int maxConcurrentUploads)
        {
            return _thunderCqrs.SendQuery(new GetCanUploadRecordQuery { MaxConcurrentUploads = maxConcurrentUploads });
        }

        /// <inheritdoc/>
        public Task UpdateConvertTrackingStatusAsync(Guid convertTrackingId, ConvertStatus status)
        {
            return _thunderCqrs.SendCommand(new UpdateConvertTrackingCommand
            {
                Id = convertTrackingId,
                Properties = new List<KeyValuePair<string, object>>
                {
                    new KeyValuePair<string, object>(nameof(ConvertingTracking.Status), status)
                }
            });
        }

        public Task UpdateFailedConvertTrackingStatusAsync(Guid convertTrackingId, FailStep failedAtStep)
        {
            return _thunderCqrs.SendCommand(new UpdateConvertTrackingCommand
            {
                Id = convertTrackingId,
                Properties = new List<KeyValuePair<string, object>>
                {
                    new KeyValuePair<string, object>(nameof(ConvertingTracking.Status), ConvertStatus.Failed),
                    new KeyValuePair<string, object>(nameof(ConvertingTracking.FailedAtStep), failedAtStep)
                }
            });
        }

        public Task MarkTrackingAsCompleteAsync(Guid convertTrackingId, string videoS3Path)
        {
            return _thunderCqrs.SendCommand(new UpdateConvertTrackingCommand
            {
                Id = convertTrackingId,
                Properties = new List<KeyValuePair<string, object>>
                {
                    new KeyValuePair<string, object>(nameof(ConvertingTracking.Status), ConvertStatus.Ready),
                    new KeyValuePair<string, object>(nameof(ConvertingTracking.S3Path), videoS3Path),
                    new KeyValuePair<string, object>(nameof(ConvertingTracking.RetryCount), 0)
                }
            });
        }

        /// <inheritdoc/>
        public async Task TrackingRecordingConvertSuccess(List<ConvertingTrackingModel> convertingTrackings)
        {
            foreach (var item in convertingTrackings)
            {
                var recordPath = $"{_recordingConvertOptions.ConvertedVideoDir}/{item.InternalMeetingId}.mp4";

                if (File.Exists(recordPath))
                {
                    await _thunderCqrs.SendCommand(new UpdateConvertTrackingCommand
                    {
                        Id = item.Id,
                        Properties = new List<KeyValuePair<string, object>>
                        {
                            new KeyValuePair<string, object>(nameof(ConvertingTracking.Status), ConvertStatus.Converted)
                        }
                    });
                }
            }
        }
    }
}
