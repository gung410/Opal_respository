using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microservice.Calendar.Application.Models;

namespace Microservice.Calendar.Application.Services
{
    public interface ICalendarEventNotifierService
    {
        Task NotifyPersonalCalendarEvent(PersonalEventDetailsModel personalEvent, CancellationToken cancellationToken = default);

        Task NotifyCommunityCalendarEvent(CommunityEventModel communityEvent, CancellationToken cancellationToken = default);
    }
}
