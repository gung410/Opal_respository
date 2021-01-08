using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microservice.Calendar.Application.Commands;
using Microservice.Calendar.Application.Models;
using Microservice.Calendar.Application.Queries;
using Microservice.Calendar.Application.RequestDtos;
using Microservice.Calendar.Domain.Enums;
using Thunder.Platform.Core.Application;
using Thunder.Platform.Core.Application.Dtos;
using Thunder.Platform.Core.Domain.Entities;
using Thunder.Platform.Core.Extensions;
using Thunder.Platform.Core.Timing;
using Thunder.Platform.Cqrs;

namespace Microservice.Calendar.Application.Services
{
    public class CommunityCalendarApplicationService : ApplicationService, ICommunityCalendarApplicationService
    {
        private readonly IThunderCqrs _thunderCqrs;
        private readonly IWebinarMeetingApplicationService _webinarMeetingApplicationService;
        private readonly ICalendarEventNotifierService _eventNotifierService;

        public CommunityCalendarApplicationService(
            IThunderCqrs thunderCqrs,
            IWebinarMeetingApplicationService webinarMeetingApplicationService,
            ICalendarEventNotifierService eventNotifierService)
        {
            _thunderCqrs = thunderCqrs;
            _webinarMeetingApplicationService = webinarMeetingApplicationService;
            _eventNotifierService = eventNotifierService;
        }

        public Task<CommunityEventModel> GetCommunityEvent(Guid id)
        {
            var query = new GetCommunityEventByIdQuery
            {
                EventId = id
            };

            return _thunderCqrs.SendQuery(query);
        }

        public async Task<CommunityEventModel> CreateWebinarCommunityEvent(CreateCommunityEventRequest request, Guid userId)
        {
            var eventId = request.Id != Guid.Empty ? request.Id : Guid.NewGuid();
            request.Id = eventId;

            var command = new CreateCommunityEventCommand
            {
                CreationRequest = request,
                Source = CalendarEventSource.CommunityWebinar,
                CreatedBy = userId,
            };

            await _thunderCqrs.SendCommand(command);

            var result = await _thunderCqrs.SendQuery(new GetCommunityEventByIdQuery { EventId = eventId });
            await _webinarMeetingApplicationService.BookWebinarMeeting(result);
            await _eventNotifierService.NotifyCommunityCalendarEvent(result);

            return result;
        }

        public Task<List<CommunityEventModel>> GetCommunityEvents(GetCommunityEventRequest request, Guid communityId)
        {
            var searchQuery = new GetCommunityEventQuery
            {
                Request = request,
                CommunityId = communityId
            };

            return _thunderCqrs.SendQuery(searchQuery);
        }

        public Task<List<CommunityEventModel>> GetCommunityEventsByUser(GetMyCommunityEventRequest request, Guid userId)
        {
            var query = new GetCommunityEventsByUserQuery
            {
                UserId = userId,
                Request = request
            };

            return _thunderCqrs.SendQuery(query);
        }

        public async Task<CommunityEventModel> CreateCommunityEvent(CreateCommunityEventRequest request, Guid userId)
        {
            var eventId = request.Id != Guid.Empty ? request.Id : Guid.NewGuid();
            request.Id = eventId;

            var command = new CreateCommunityEventCommand
            {
                CreationRequest = request,
                Source = CalendarEventSource.CommunityRegular,
                CreatedBy = userId
            };

            await _thunderCqrs.SendCommand(command);

            var result = await _thunderCqrs.SendQuery(new GetCommunityEventByIdQuery { EventId = eventId });
            await _eventNotifierService.NotifyCommunityCalendarEvent(result);

            return result;
        }

        public async Task<CommunityEventModel> UpdateCommunityEvent(UpdateCommunityEventRequest request, Guid userId)
        {
            bool canUpdateEvent = await CheckCanUpdateCommunityEvent(request.Id, userId);
            if (!canUpdateEvent)
            {
                throw new EntityNotFoundException();
            }

            var command = new UpdateCommunityEventCommand
            {
                UserId = userId,
                Request = request
            };

            await _thunderCqrs.SendCommand(command);

            var result = await _thunderCqrs.SendQuery(new GetCommunityEventByIdQuery { EventId = request.Id });
            await _eventNotifierService.NotifyCommunityCalendarEvent(result);

            return result;
        }

        public async Task DeleteCommunityEvent(Guid eventId, Guid userId)
        {
            bool canDeleteEvent = await CheckCanUpdateCommunityEvent(eventId, userId);
            if (!canDeleteEvent)
            {
                throw new EntityNotFoundException();
            }

            var command = new DeleteCommunityEventCommand
            {
                EventId = eventId,
                UserId = userId
            };

            await _thunderCqrs.SendCommand(command);
        }

        public Task<PagedResultDto<CommunityEventModel>> GetCommunityEventsByCommunityId(Guid userId, GetCommunityEventsByCommunityIdRequest request)
        {
            var query = new GetCommunityEventsByCommunityIdQuery
            {
                Request = request,
                UserId = userId,
                PageInfo = new PagedResultRequestDto
                {
                    MaxResultCount = request.MaxResultCount,
                    SkipCount = request.SkipCount
                }
            };

            return _thunderCqrs.SendQuery(query);
        }

        public Task<CommunityEventDetailsModel> GetCommunityEventDetails(Guid eventId, Guid userId)
        {
            var query = new GetCommunityEventDetailsByIdQuery
            {
                EventId = eventId,
                UserId = userId
            };

            return _thunderCqrs.SendQuery(query);
        }

        public async Task<CommunityEventModel> UpdateWebinarCommunityEvent(UpdateCommunityEventRequest request, Guid userId)
        {
            bool canUpdateEvent = await CheckCanUpdateCommunityEvent(request.Id, userId);
            if (!canUpdateEvent)
            {
                throw new EntityNotFoundException();
            }

            var command = new UpdateCommunityEventCommand
            {
                UserId = userId,
                Request = request
            };

            await _thunderCqrs.SendCommand(command);

            var result = await _thunderCqrs.SendQuery(new GetCommunityEventByIdQuery { EventId = request.Id });
            await _webinarMeetingApplicationService.UpdateWebinarMeeting(result);
            await _eventNotifierService.NotifyCommunityCalendarEvent(result);

            return result;
        }

        public async Task DeleteWebinarCommunityEvent(Guid eventId, Guid userId)
        {
            bool canDeleteEvent = await CheckCanUpdateCommunityEvent(eventId, userId);
            if (!canDeleteEvent)
            {
                throw new EntityNotFoundException();
            }

            var command = new DeleteCommunityEventCommand
            {
                EventId = eventId,
                UserId = userId
            };
            await _thunderCqrs.SendCommand(command);

            await _webinarMeetingApplicationService.CancelWebinarMeeting(eventId);
        }

        private async Task<bool> CheckCanUpdateCommunityEvent(Guid eventId, Guid userId)
        {
            var query = new GetOwnCommunityQuery
            {
                UserId = userId
            };
            var ownCommunities = await _thunderCqrs.SendQuery(query);

            var communityEvent = await GetCommunityEvent(eventId);
            if (communityEvent.Source == CalendarEventSource.CommunityRegular)
            {
                return ownCommunities.Any(x => x.Id == communityEvent.CommunityId);
            }

            return ownCommunities.Any(x => x.Id == communityEvent.CommunityId) &&
                    communityEvent.StartAt > Clock.Now.ToDateTimeInSystemTimeZone().ToUtcFromSystemTimeZone();
        }
    }
}
