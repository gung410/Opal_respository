using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microservice.Calendar.Application.Commands;
using Microservice.Calendar.Application.Models;
using Microservice.Calendar.Application.Queries;
using Microservice.Calendar.Application.RequestDtos;
using Microservice.Calendar.Domain.Enums;
using Microservice.Calendar.Domain.Utilities;
using Thunder.Platform.Core.Application;
using Thunder.Platform.Cqrs;

namespace Microservice.Calendar.Application.Services
{
    public class PersonalCalendarApplicationService : ApplicationService, IPersonalCalendarApplicationService
    {
        private readonly IThunderCqrs _thunderCqrs;
        private readonly ICalendarEventNotifierService _eventNotificationApplicationService;

        public PersonalCalendarApplicationService(
            IThunderCqrs thunderCqrs,
            ICalendarEventNotifierService eventNotificationApplicationService)
        {
            _thunderCqrs = thunderCqrs;
            _eventNotificationApplicationService = eventNotificationApplicationService;
        }

        public Task<List<EventModel>> GetPersonalCalendarEventsByRange(GetPersonalEventByRangeRequest request, Guid userId)
        {
            var query = new GetPersonalEventByRangeQuery
            {
                UserId = userId,
                Request = request
            };
            return _thunderCqrs.SendQuery(query);
        }

        public Task<int> CountPersonalCalendarEventsByRange(GetPersonalEventByRangeRequest request, Guid userId)
        {
            var query = new CountPersonalEventByRangeQuery
            {
                UserId = userId,
                Request = request
            };
            return _thunderCqrs.SendQuery(query);
        }

        public Task<List<EventModel>> GetPersonalCalendarEvents(GetPersonalEventRequest request, Guid userId)
        {
            var rangeStart = request
                .OffsetPoint
                .AddMonths(-1 * request.NumberMonthOffset);

            var rangeEnd = request.OffsetPoint
                .AddMonths(request.NumberMonthOffset)
                .ToLastDayOfMonth();

            var query = new GetPersonalEventByRangeQuery
            {
                UserId = userId,
                Request = new GetPersonalEventByRangeRequest
                {
                    StartAt = rangeStart,
                    EndAt = rangeEnd
                }
            };

            return _thunderCqrs.SendQuery(query);
        }

        public Task<PersonalEventDetailsModel> GetEventDetails(Guid id, Guid userId)
        {
            var query = new GetPersonalEventDetailsByIdQuery
            {
                EventId = id,
                UserId = userId
            };

            return _thunderCqrs.SendQuery(query);
        }

        public async Task<PersonalEventModel> CreateEvent(CreatePersonalEventRequest request, Guid userId)
        {
            var eventId = request.Id != Guid.Empty ? request.Id : Guid.NewGuid();
            request.Id = eventId;

            var command = new CreatePersonalEventCommand
            {
                CreationRequest = request,
                Source = CalendarEventSource.SelfCreated,
                SourceId = null,
                CreatedBy = userId,
                UserId = userId,
            };

            await _thunderCqrs.SendCommand(command);

            var result = await _thunderCqrs.SendQuery(new GetPersonalEventDetailsByIdQuery { EventId = eventId, UserId = userId });
            await _eventNotificationApplicationService.NotifyPersonalCalendarEvent(result);

            return result;
        }

        public async Task<PersonalEventModel> UpdateEvent(UpdatePersonalEventRequest request, Guid userId)
        {
            var command = new UpdatePersonalEventCommand
            {
                UserId = userId,
                Request = request
            };

            await _thunderCqrs.SendCommand(command);

            var result = await _thunderCqrs.SendQuery(new GetPersonalEventDetailsByIdQuery { EventId = request.Id, UserId = userId });
            await _eventNotificationApplicationService.NotifyPersonalCalendarEvent(result);

            return result;
        }

        public Task DeleteEvent(Guid eventId, Guid userId)
        {
            var command = new DeletePersonalEventCommand
            {
                EventId = eventId,
                UserId = userId
            };

            return _thunderCqrs.SendCommand(command);
        }
    }
}
