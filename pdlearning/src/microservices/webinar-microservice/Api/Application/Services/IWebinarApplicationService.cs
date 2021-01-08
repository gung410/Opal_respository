using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microservice.Webinar.Application.Models;
using Microservice.Webinar.Application.RequestDtos;
using Microservice.Webinar.Domain.Enums;

namespace Microservice.Webinar.Application.Services
{
    public interface IWebinarApplicationService
    {
        /// <summary>
        /// When the meeting exists and available, the user has permission to access the meeting.
        /// Create meeting and get join URL of the meeting on BigBlueButton.
        /// </summary>
        /// <param name="sourceId">SourceId of the Booking.</param>
        /// <param name="source">Source of the Booking.</param>
        /// <param name="userId">UserId of the Attendee.</param>
        /// <returns>Information of the join URL.</returns>
        Task<ResultGetJoinUrlModel> GetJoinUrl(Guid sourceId, BookingSource source, Guid userId);

        /// <summary>
        /// To save information of the book meeting include: booking, meeting, attendees.
        /// </summary>
        /// <param name="request">The required params to book meeting.</param>
        /// <returns>.</returns>
        Task BookMeeting(BookMeetingRequest request);

        /// <summary>
        /// Get Meeting PreRecordings.
        /// </summary>
        /// <param name="meetingIds">meeting ids.</param>
        /// <returns>meetingId and prerecording URL.</returns>
        Task<List<ResultGetMeetingPreRecordingModel>> GetMeetingPreRecordings(List<Guid> meetingIds);

        /// <summary>
        /// Cancel the meeting by update 'Canceled' value.
        /// </summary>
        /// <param name="request">The required params to cancel meeting.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task CancelMeeting(CancelMeetingRequest request);

        /// <summary>
        /// Update information of the meeting include: booking, meeting, attendees.
        /// </summary>
        /// <param name="request">The required params to update meeting.</param>
        /// <returns>.</returns>
        Task UpdateMeeting(UpdateMeetingRequest request);

        /// <summary>
        /// Check if the meeting exists.
        /// </summary>
        /// <param name="sessionId">Meeting's session ID.</param>
        /// <param name="source">Meeting's source.</param>
        /// <returns>True if exists.</returns>
        Task<bool> CheckBookingExistsAsync(Guid sessionId, BookingSource source);
    }
}
