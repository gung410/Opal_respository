using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microservice.Calendar.Application.Models;
using Microservice.Calendar.Application.RequestDtos;

namespace Microservice.Calendar.Application.Services
{
    public interface IPersonalCalendarApplicationService
    {
        Task<List<EventModel>> GetPersonalCalendarEventsByRange(GetPersonalEventByRangeRequest request, Guid userId);

        Task<int> CountPersonalCalendarEventsByRange(GetPersonalEventByRangeRequest request, Guid userId);

        Task<List<EventModel>> GetPersonalCalendarEvents(GetPersonalEventRequest request, Guid userId);

        Task<PersonalEventDetailsModel> GetEventDetails(Guid id, Guid userId);

        Task<PersonalEventModel> UpdateEvent(UpdatePersonalEventRequest request, Guid userId);

        Task DeleteEvent(Guid eventId, Guid userId);

        Task<PersonalEventModel> CreateEvent(CreatePersonalEventRequest request, Guid userId);
    }
}
