using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microservice.WebinarVideoConverter.Application.Models;
using Microservice.WebinarVideoConverter.Domain.Entities;
using Microservice.WebinarVideoConverter.Domain.Enums;

namespace Microservice.WebinarVideoConverter.Application.Services.Abstractions
{
    public interface IConvertingTrackingService
    {
        /// <summary>
        /// Get converting tracking by status.
        /// </summary>
        /// <param name="status">Convert status.</param>
        /// <returns>A list of <see cref="ConvertingTrackingModel"/>.</returns>
        Task<List<ConvertingTrackingModel>> GetConvertingTrackingsByStatus(ConvertStatus status);

        /// <summary>
        /// Update convert tracking status.
        /// </summary>
        /// <param name="convertTrackingId"><see cref="ConvertingTracking.Id"/>.</param>
        /// <param name="status"><see cref="ConvertStatus"/>.</param>
        /// <returns><see cref="Task.CompletedTask"/>.</returns>
        Task UpdateConvertTrackingStatusAsync(Guid convertTrackingId, ConvertStatus status);

        /// <summary>
        /// Sets status to Failed and FailedAtStep.
        /// </summary>
        /// <param name="convertTrackingId">The object ID.</param>
        /// <param name="failedAtStep">The status identifies which step has failed at.</param>
        /// <returns><see cref="Task"/>.</returns>
        Task UpdateFailedConvertTrackingStatusAsync(Guid convertTrackingId, FailStep failedAtStep);

        Task MarkTrackingAsCompleteAsync(Guid convertTrackingId, string videoS3Path);

        /// <summary>
        /// Inserts converting tracking if the internal meeting id not exists in the database.
        /// </summary>
        /// <param name="trackings"><see cref="ConvertingTracking"/> entities.</param>
        /// <returns>Task.</returns>
        Task InsertIfNotExistsAsync(IEnumerable<ConvertingTracking> trackings);

        /// <summary>
        /// Gets <see cref="ConvertingTracking"/> entities with status = Failed.
        /// </summary>
        /// <returns><see cref="ConvertingTracking"/> entities with status = Failed.</returns>
        Task<List<ConvertingTracking>> GetFailedTrackings();

        /// <summary>
        /// Processes <see cref="ConvertingTracking"/> objects to make them retryable.
        /// </summary>
        /// <param name="convertingTrackingId">Id of converting tracking id.</param>
        /// <returns><see cref="Task"/>.</returns>
        Task ProcessRetryRecord(Guid convertingTrackingId);

        Task<List<ConvertingTrackingModel>> GetCanUploadRecord(int maxConcurrentUploads);

        /// <summary>
        /// Update convert tracking status when convert is success.
        /// </summary>
        /// <param name="convertingTrackings">A list of <see cref="ConvertingTrackingModel"/>.</param>
        /// <returns><see cref="Task.CompletedTask"/>.</returns>
        Task TrackingRecordingConvertSuccess(List<ConvertingTrackingModel> convertingTrackings);

        /// <summary>
        /// Processes <see cref="ConvertingTracking"/> objects to make them to be ignored to retry.
        /// </summary>
        /// <param name="convertingTrackingId">Id of <see cref="ConvertingTrackingModel"/> to be made ignored to retry.</param>
        /// <returns><see cref="Task"/>.</returns>
        Task ProcessIgnoreRetryRecord(Guid convertingTrackingId);
    }
}
