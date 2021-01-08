using System;
using Microservice.Calendar.Domain.Entities;
using Microservice.Calendar.Domain.Enums;

namespace Microservice.Calendar.Domain.Extensions
{
    public static class EventExtensions
    {
        public static TEvent From<TEvent>(this TEvent @event, DateTime startAt) where TEvent : EventEntity
        {
            @event.StartAt = startAt;
            return @event;
        }

        public static TEvent To<TEvent>(this TEvent @event, DateTime endAt) where TEvent : EventEntity
        {
            @event.EndAt = endAt;
            return @event;
        }

        public static TEvent WithStatus<TEvent>(this TEvent @event, EventStatus status = EventStatus.Opening) where TEvent : EventEntity
        {
            @event.Status = status;
            return @event;
        }

        public static TEvent WithTitle<TEvent>(this TEvent @event, string title) where TEvent : EventEntity
        {
            @event.Title = title;
            return @event;
        }

        public static TEvent WithBasicInfo<TEvent>(this TEvent @event, string title, string description) where TEvent : EventEntity
        {
            @event.Title = title;
            @event.Description = description;

            return @event;
        }

        public static TEvent WithBasicInfo<TEvent>(this TEvent @event, string title, string description, Guid communityId) where TEvent : CommunityEvent
        {
            @event.Title = title;
            @event.Description = description;
            @event.CommunityId = communityId;

            return @event;
        }

        public static TEvent WithTime<TEvent>(this TEvent @event, DateTime startAt, DateTime endAt, bool isAllDay = false) where TEvent : EventEntity
        {
            @event.StartAt = startAt;
            @event.EndAt = endAt;
            @event.IsAllDay = isAllDay;
            return @event;
        }

        public static TEvent WithTime<TEvent>(this TEvent @event, DateTime startAt, DateTime endAt, DateTime? repeatUntil, RepeatFrequency repeatFrequency, bool isAllDay = false) where TEvent : EventEntity
        {
            @event.StartAt = startAt;
            @event.EndAt = endAt;
            @event.IsAllDay = isAllDay;
            @event.RepeatFrequency = repeatFrequency;
            @event.RepeatUntil = repeatUntil;
            return @event;
        }

        public static TEvent WithOwner<TEvent>(this TEvent @event, Guid? ownerId) where TEvent : EventEntity
        {
            if (ownerId.HasValue)
            {
                @event.CreatedBy = ownerId.Value;
            }

            return @event;
        }

        public static TEvent FromSource<TEvent>(this TEvent @event, CalendarEventSource source, Guid? sourceId) where TEvent : EventEntity
        {
            @event.Source = source;
            @event.SourceId = sourceId;

            return @event;
        }

        public static TEvent FromSourceParent<TEvent>(this TEvent @event, Guid sourceParentId) where TEvent : EventEntity
        {
            @event.SourceParentId = sourceParentId;
            return @event;
        }

        public static TEvent WithPrivacy<TEvent>(this TEvent @event, CommunityEventPrivacy? privacy) where TEvent : CommunityEvent
        {
            if (privacy.HasValue)
            {
                @event.CommunityEventPrivacy = privacy.Value;
            }

            return @event;
        }
    }
}
