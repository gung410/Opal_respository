using System;
using System.Threading.Tasks;
using Microservice.Calendar.Application.Models;

namespace Microservice.Calendar.Application.Services
{
    public interface IWebinarMeetingApplicationService
    {
        Task BookWebinarMeeting(CommunityEventModel eventModel);

        Task UpdateWebinarMeeting(CommunityEventModel eventModel);

        Task CancelWebinarMeeting(Guid eventId);

        /// <summary>
        /// Send event for update meeting to Webinar service.
        /// </summary>
        /// <param name="communityId">Community Id.</param>
        /// <returns>.</returns>
        Task UpdateWebinarMeetingsByCommunityId(Guid communityId);
    }
}
