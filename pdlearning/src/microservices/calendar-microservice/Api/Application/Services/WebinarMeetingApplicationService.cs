using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microservice.Calendar.Application.Events.WebinarBooking;
using Microservice.Calendar.Application.Models;
using Microservice.Calendar.Application.RequestDtos;
using Microservice.Calendar.Application.WebinarBooking;
using Microservice.Calendar.Domain.Entities;
using Microservice.Calendar.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Application;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Timing;
using Thunder.Platform.Cqrs;

namespace Microservice.Calendar.Application.Services
{
    public class WebinarMeetingApplicationService : ApplicationService, IWebinarMeetingApplicationService
    {
        private const int PageSize = 10;
        private readonly IThunderCqrs _thunderCqrs;
        private readonly IRepository<CommunityMembership> _communityMembershipRepository;
        private readonly IRepository<CommunityEvent> _communityEventRepostirory;

        public WebinarMeetingApplicationService(
            IThunderCqrs thunderCqrs,
            IRepository<CommunityMembership> communityMembershipRepository,
            IRepository<CommunityEvent> communityEventRepostirory)
        {
            _thunderCqrs = thunderCqrs;
            _communityMembershipRepository = communityMembershipRepository;
            _communityEventRepostirory = communityEventRepostirory;
        }

        public async Task BookWebinarMeeting(CommunityEventModel eventModel)
        {
            var request = await CreateMeetingWebinar(eventModel);

            await _thunderCqrs.SendEvent(new WebinarMeetingEvent(request, WebinarMeetingAction.Book));
        }

        public async Task UpdateWebinarMeeting(CommunityEventModel eventModel)
        {
            var request = await CreateMeetingWebinar(eventModel);

            await _thunderCqrs.SendEvent(new WebinarMeetingEvent(request, WebinarMeetingAction.Update));
        }

        public async Task UpdateWebinarMeetingsByCommunityId(Guid communityId)
        {
            var pageCount = 0;
            var resultCount = PageSize;
            while (resultCount == PageSize)
            {
                var availableEvents = await _communityEventRepostirory
                    .GetAll()
                    .Where(x => x.Source == CalendarEventSource.CommunityWebinar && x.CommunityId == communityId)
                    .Where(x => DateTime.Compare(x.EndAt, Clock.Now) > 0)
                    .Skip(pageCount * PageSize)
                    .Take(PageSize)
                    .ToListAsync();

                resultCount = availableEvents.Count();
                pageCount++;

                if (resultCount > 0)
                {
                    var attendees = await _communityMembershipRepository
                       .GetAll()
                       .Where(x => x.CommunityId == communityId)
                       .Select(p => new AttendeeInfoRequest
                       {
                           Id = p.UserId,
                           IsModerator = p.Role == CommunityMembershipRole.Admin || p.Role == CommunityMembershipRole.Owner
                       })
                       .ToListAsync();

                    if (attendees.Any())
                    {
                        var events = new List<BaseThunderEvent>();
                        foreach (var item in availableEvents)
                        {
                            var updateMeetingEvent = new WebinarMeetingEvent(
                                new WebinarMeetingRequest
                                {
                                    SessionId = item.Id,
                                    Title = item.Title,
                                    StartTime = item.StartAt,
                                    EndTime = item.EndAt,
                                    Source = WebinarBookingSource.Community,
                                    Attendees = attendees
                                },
                                WebinarMeetingAction.Update);

                            events.Add(updateMeetingEvent);
                        }

                        await _thunderCqrs.SendEvents(events);
                    }
                }
            }
        }

        public async Task CancelWebinarMeeting(Guid eventId)
        {
            await _thunderCqrs.SendEvent(
                new WebinarMeetingEvent(
                new WebinarMeetingRequest
                {
                    SessionId = eventId,
                    Source = WebinarBookingSource.Community
                },
                WebinarMeetingAction.Cancel));
        }

        private async Task<WebinarMeetingRequest> CreateMeetingWebinar(CommunityEventModel eventModel)
        {
            // Get all member from the community
            var membership = await _communityMembershipRepository.GetAll()
                .Where(x => x.CommunityId == eventModel.CommunityId)
                .Select(x => new AttendeeInfoRequest
                {
                    Id = x.UserId,
                    IsModerator = x.Role == CommunityMembershipRole.Owner || x.Role == CommunityMembershipRole.Admin
                })
                .Distinct()
                .ToListAsync();

            return new WebinarMeetingRequest()
            {
                SessionId = eventModel.Id,
                Title = eventModel.Title,
                StartTime = eventModel.StartAt,
                EndTime = eventModel.EndAt,
                Source = WebinarBookingSource.Community,
                Attendees = membership
            };
        }
    }
}
