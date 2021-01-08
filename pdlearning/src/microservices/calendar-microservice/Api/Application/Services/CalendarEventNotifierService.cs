using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Conexus.Opal.Connector.RabbitMQ.Contract;
using Microservice.Calendar.Application.Events;
using Microservice.Calendar.Application.Events.WebinarBooking;
using Microservice.Calendar.Application.Models;
using Microservice.Calendar.Application.RequestDtos;
using Microservice.Calendar.Application.WebinarBooking;
using Microservice.Calendar.Domain.Entities;
using Microservice.Calendar.Domain.Enums;
using Microservice.Calendar.Infrastructure.Helpers;
using Microservice.Calendar.Infrastructure.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Thunder.Platform.Core.Application;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Timing;
using Thunder.Platform.Cqrs;

namespace Microservice.Calendar.Application.Services
{
    public class CalendarEventNotifierService : ICalendarEventNotifierService
    {
        private readonly IThunderCqrs _thunderCqrs;
        private readonly IRepository<CommunityMembership> _communityMembershipRepository;
        private readonly NotificationTimeZoneOption _systemTimeZoneOption;

        public CalendarEventNotifierService(
            IThunderCqrs thunderCqrs,
            IRepository<CommunityMembership> communityMembershipRepository,
            IOptions<NotificationTimeZoneOption> systemTimeZoneOption)
        {
            _thunderCqrs = thunderCqrs;
            _communityMembershipRepository = communityMembershipRepository;
            _systemTimeZoneOption = systemTimeZoneOption.Value;
        }

        public async Task NotifyCommunityCalendarEvent(CommunityEventModel communityEvent, CancellationToken cancellationToken = default)
        {
            // Get all member from the community
            var membership = await _communityMembershipRepository
                .GetAll()
                .Where(x => x.CommunityId == communityEvent.CommunityId && x.Id != communityEvent.CreatedBy)
                .ToListAsync();

            if (!membership.Any())
            {
                return;
            }

            // Send email notification
            var newInvitationEvents = membership
                .Select(x => x.UserId)
                .Distinct()
                .Select(memberId => new NotifyInvitationEvent(
                    new NotifyInvitationPayload(
                        communityEvent.Title,
                        TimeZoneInfo.ConvertTimeFromUtc(communityEvent.StartAt, TimeHelper.GetSystemTimeZoneInfo(_systemTimeZoneOption)),
                        TimeZoneInfo.ConvertTimeFromUtc(communityEvent.EndAt, TimeHelper.GetSystemTimeZoneInfo(_systemTimeZoneOption)),
                        communityEvent.RepeatUntil,
                        communityEvent.IsAllDay,
                        communityEvent.RepeatFrequency),
                    communityEvent.Id,
                    communityEvent.CreatedBy.Value,
                    memberId,
                    new ReminderByDto
                    {
                        Type = ReminderByType.AbsoluteDateTimeUTC,
                        Value = Clock.Now.ToString("MM/dd/yyyy HH:mm:ss")
                    }))
                .ToList();

            await _thunderCqrs.SendEvents(newInvitationEvents, cancellationToken);
        }

        public async Task NotifyPersonalCalendarEvent(PersonalEventDetailsModel personalEvent, CancellationToken cancellationToken = default)
        {
            if (!personalEvent.AttendeeIds.Any())
            {
                return;
            }

            var newInvitationEvents = personalEvent.AttendeeIds
                .Where(aId => aId != personalEvent.CreatedBy)
                .Distinct()
                .Select(attendeeId => new NotifyInvitationEvent(
                    new NotifyInvitationPayload(
                        personalEvent.Title,
                        TimeZoneInfo.ConvertTimeFromUtc(personalEvent.StartAt, TimeHelper.GetSystemTimeZoneInfo(_systemTimeZoneOption)),
                        TimeZoneInfo.ConvertTimeFromUtc(personalEvent.EndAt, TimeHelper.GetSystemTimeZoneInfo(_systemTimeZoneOption)),
                        personalEvent.RepeatUntil,
                        personalEvent.IsAllDay,
                        personalEvent.RepeatFrequency),
                    personalEvent.Id,
                    personalEvent.CreatedBy.Value,
                    attendeeId,
                    new ReminderByDto
                    {
                        Type = ReminderByType.AbsoluteDateTimeUTC,
                        Value = Clock.Now.ToString("MM/dd/yyyy HH:mm:ss")
                    }))
                .ToList();

            await _thunderCqrs.SendEvents(newInvitationEvents, cancellationToken);
        }
    }
}
