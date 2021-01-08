using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LearnerApp.Models.Calendar;
using Refit;

namespace LearnerApp.Services.Backend
{
    public interface ICalendarBackendService
    {
        [Get("/api/calendars/personal/myEvents/range?startAt={startAt}&endAt={endAt}")]
        Task<List<CalendarEvent>> GetListCalendarEvent(DateTime startAt, DateTime endAt);

        [Get("/api/calendars/personal/myEvents/range/count?startAt={startAt}&endAt={endAt}")]
        Task<int> GetCalendarEventCount(DateTime startAt, DateTime endAt);
    }
}
