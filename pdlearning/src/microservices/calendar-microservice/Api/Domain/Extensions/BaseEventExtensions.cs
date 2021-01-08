using System;
using System.Linq;
using Microservice.Calendar.Domain.Entities;
using Microservice.Calendar.Domain.Enums;

namespace Microservice.Calendar.Domain.Extensions
{
    public static class BaseEventExtensions
    {
        /// <summary>
        /// Filter selected field from start to end, inclusively.
        /// </summary>
        /// <typeparam name="TEvent">Event type.</typeparam>
        /// <param name="query"><see cref="PersonalEvent"/> query.</param>
        /// <param name="start">Start DateTime.</param>
        /// <param name="end">End DateTime.</param>
        /// <returns>Filtered <see cref="PersonalEvent"/> query.</returns>
        public static IQueryable<TEvent> StartAtBetween<TEvent>(
            this IQueryable<TEvent> query,
            DateTime start,
            DateTime end)
            where TEvent : EventEntity
        {
            return query.Where(pe => pe.StartAt >= start && pe.StartAt <= end);
        }

        /// <summary>
        /// Filter which event overlaps DateTime range.
        /// </summary>
        /// <typeparam name="TEvent">Base event <see cref="EventEntity"/>.</typeparam>
        /// <param name="query">Event source query.</param>
        /// <param name="start">Range start.</param>
        /// <param name="end">Range end.</param>
        /// <returns>Filtered overlapped events.</returns>
        public static IQueryable<TEvent> OverlapsDateTimeRange<TEvent>(
            this IQueryable<TEvent> query,
            DateTime start,
            DateTime end)
            where TEvent : EventEntity
        {
            return query.Where(e => e.StartAt <= end && e.EndAt >= start);
        }

        /// <summary>
        /// Filter available events with status is Opening.
        /// </summary>
        /// <typeparam name="TEvent">Base event <see cref="EventEntity"/>.</typeparam>
        /// <param name="query">Event source query.</param>
        /// <returns>List available event.</returns>
        public static IQueryable<TEvent> GetAvailableEvents<TEvent>(
            this IQueryable<TEvent> query) where TEvent : EventEntity
        {
            return query.Where(e => e.Status == EventStatus.Opening);
        }
    }
}
