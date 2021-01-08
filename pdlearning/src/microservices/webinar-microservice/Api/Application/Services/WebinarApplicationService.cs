using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microservice.Webinar.Application.Commands;
using Microservice.Webinar.Application.Exception;
using Microservice.Webinar.Application.Models;
using Microservice.Webinar.Application.Queries;
using Microservice.Webinar.Application.RequestDtos;
using Microservice.Webinar.Application.Services.BigBlueButton;
using Microservice.Webinar.Domain.Entities;
using Microservice.Webinar.Domain.Enums;
using Microsoft.Extensions.Options;
using Thunder.Platform.Core.Application;
using Thunder.Platform.Core.Domain.Entities;
using Thunder.Platform.Cqrs;

namespace Microservice.Webinar.Application.Services
{
    public class WebinarApplicationService : ApplicationService, IWebinarApplicationService
    {
        private readonly IBigBlueButtonService _bigBlueButtonService;
        private readonly IThunderCqrs _thunderCqrs;
        private readonly BigBlueButtonServerOptions _bigBlueButtonServerOpts;

        public WebinarApplicationService(IBigBlueButtonService bigBlueButtonService, IThunderCqrs thunderCqrs, IOptions<BigBlueButtonServerOptions> bigBlueButtonServerOpts)
        {
            _bigBlueButtonService = bigBlueButtonService;
            _thunderCqrs = thunderCqrs;
            _bigBlueButtonServerOpts = bigBlueButtonServerOpts.Value;
        }

        /// <inheritdoc/>
        public async Task BookMeeting(BookMeetingRequest request)
        {
            var meetingId = Guid.NewGuid();

            // Add Meeting
            var meetingInfoCommand = new SaveMeetingInfoCommand
            {
                Id = meetingId,
                Title = request.Title,
                PreRecordPath = request.PreRecordPath,
                PreRecordId = request.PreRecordId ?? null,
                StartTime = request.StartTime,
                EndTime = request.EndTime,
                ParticipantCount = request.Attendees.Count,
                IsCanceled = false
            };
            await _thunderCqrs.SendCommand(meetingInfoCommand);

            // Add Attendees
            var attendeeList = request.Attendees
                .Select(p => new SaveAttendeesCommandItem
                {
                    MeetingId = meetingId,
                    UserId = p.Id,
                    IsModerator = p.IsModerator
                }).ToList();

            var attendeeCommand = new SaveAttendeesCommand
            {
                Attendees = attendeeList
            };
            await _thunderCqrs.SendCommand(attendeeCommand);

            // Add Booking
            var bookingCommand = new SaveBookingCommand()
            {
                SourceId = request.SessionId,
                MeetingId = meetingId,
                Source = request.Source
            };
            await _thunderCqrs.SendCommand(bookingCommand);
        }

        public async Task<List<ResultGetMeetingPreRecordingModel>> GetMeetingPreRecordings(List<Guid> meetingIds)
        {
            return await _thunderCqrs.SendQuery(new GetMeetingPreRecordingQuery { MeetingIds = meetingIds });
        }

        /// <inheritdoc/>
        public async Task<ResultGetJoinUrlModel> GetJoinUrl(Guid sourceId, BookingSource source, Guid userId)
        {
            var user = await _thunderCqrs.SendQuery(new GetUserByIdQuery { Id = userId });

            if (user == null)
            {
                throw new EntityNotFoundException(typeof(WebinarUser), userId);
            }

            var meetingInfo = await _thunderCqrs.SendQuery(new GetMeetingBySourceQuery { SourceId = sourceId, Source = source });
            if (meetingInfo == null || meetingInfo.IsCanceled)
            {
                throw new MeetingNotFoundException();
            }

            // Check user join meeting
            var attendee = await _thunderCqrs.SendQuery(new GetAttendeeQuery { MeetingId = meetingInfo.Id, UserId = userId });
            if (attendee == null)
            {
                throw new MeetingAccessDeniedException();
            }

            await ValidateMeetingTime(meetingInfo);

            // Return to waiting page server due to no BBB server is available for the meeting.
            if (string.IsNullOrEmpty(meetingInfo.BBBServerPrivateIp))
            {
                return new ResultGetJoinUrlModel
                {
                    IsSuccess = true,
                    JoinUrl = $"{_bigBlueButtonServerOpts.WaitingServerPageUrl}/{source}/{sourceId}"
                };
            }

            // If meeting is valid. We'll request to ScaleLite for creating meeting and return the URL for user.
            var createResult = await _bigBlueButtonService.CreateMeeting(new CreateMeetingRequest
            {
                MeetingId = meetingInfo.Id,
                MeetingName = meetingInfo.Title,
                BBBServerPrivateIp = meetingInfo.BBBServerPrivateIp
            });

            if (!createResult.IsSuccess)
            {
                return new ResultGetJoinUrlModel
                {
                    IsSuccess = false,
                    Message = createResult.Message,
                    MessageCode = createResult.MessageCode
                };
            }

            var joinRequest = new JoinMeetingRequest
            {
                BBBServerPrivateIp = meetingInfo.BBBServerPrivateIp,
                IsModerator = attendee.IsModerator,
                FullName = user.FullName,
                UserId = user.Id,
                MeetingId = meetingInfo.Id.ToString(),
                Redirect = true
            };

            return new ResultGetJoinUrlModel
            {
                IsSuccess = true,
                JoinUrl = _bigBlueButtonService.GetJoinUrl(joinRequest)
            };
        }

        /// <inheritdoc/>
        public async Task CancelMeeting(CancelMeetingRequest request)
        {
            var cancelMeetingCommand = new CancelMeetingCommand
            {
                SourceId = request.SessionId,
                Source = request.Source
            };

            await _thunderCqrs.SendCommand(cancelMeetingCommand);
        }

        public Task<bool> CheckBookingExistsAsync(Guid sessionId, BookingSource source)
        {
            var meetingCheckQuery = new CheckBookingExistsQuery
            {
                Source = source,
                SessionId = sessionId
            };

            return _thunderCqrs.SendQuery(meetingCheckQuery);
        }

        /// <inheritdoc/>
        public async Task UpdateMeeting(UpdateMeetingRequest request)
        {
            // Update Meeting
            var meetingQuery = new GetMeetingBySourceQuery { SourceId = request.SessionId, Source = request.Source };
            var meetingInfo = await _thunderCqrs.SendQuery(meetingQuery);

            if (meetingInfo == null)
            {
                throw new MeetingNotFoundException();
            }

            var meetingCommand = new UpdateMeetingInfoCommand
            {
                Id = meetingInfo.Id,
                Title = request.Title,
                StartTime = request.StartTime,
                EndTime = request.EndTime,
                PreRecordPath = request.PreRecordPath,
                PreRecordId = request.PreRecordId ?? null,
                ParticipantCount = request.Attendees.Count()
            };
            await _thunderCqrs.SendCommand(meetingCommand);

            // Update Attendees
            var attendeeList = request.Attendees.Select(p => new UpdateAttendeeCommandItem
            {
                IsModerator = p.IsModerator,
                MeetingId = meetingInfo.Id,
                UserId = p.Id
            }).ToList();
            var attendeesCommand = new UpdateAttendeesCommand
            {
                MeetingId = meetingInfo.Id,
                Attendees = attendeeList
            };
            await _thunderCqrs.SendCommand(attendeesCommand);
        }

        private async Task ValidateMeetingTime(MeetingInfoModel meetingInfo)
        {
            // Check Meeting time has valid time to join
            var checkMeetingRequest = new CheckMeetingAvailableRequest
            {
                MeetingId = meetingInfo.Id,
                BBBServerPrivateIp = meetingInfo.BBBServerPrivateIp,
                StartTime = meetingInfo.StartTime,
                EndTime = meetingInfo.EndTime
            };

            var isValidMeetingTime = _bigBlueButtonService.ValidateMeetingTime(checkMeetingRequest);
            if (!isValidMeetingTime)
            {
                var hasAttendeeRemainInMeeting = await _bigBlueButtonService.HasAttendeeRemainInMeeting(checkMeetingRequest);
                if (!hasAttendeeRemainInMeeting)
                {
                    throw new InvalidMeetingTimeException();
                }
            }
        }
    }
}
