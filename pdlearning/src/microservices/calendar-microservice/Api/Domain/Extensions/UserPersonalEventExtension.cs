using System;
using System.Linq;
using Microservice.Calendar.Domain.Entities;

namespace Microservice.Calendar.Domain.Extensions
{
    public static class UserPersonalEventExtension
    {
        /// <summary>
        /// Gets events that either user is owner or user is attending.
        /// </summary>
        /// <param name="personalEventQueryable">Personal event queryable.</param>
        /// <param name="userId">The Id of User.</param>
        /// <returns>IQueryable of UserPersonalEvent.</returns>
        public static IQueryable<UserPersonalEvent> GetOwnerOrAcceptedEvents(
            this IQueryable<UserPersonalEvent> personalEventQueryable,
            Guid userId)
        {
            return personalEventQueryable
                .Where(upe => upe.UserId == userId)
                .Where(upe => upe.Event.CreatedBy == userId || upe.IsAccepted);
        }

        /// <summary>
        /// Get events that this user is attending.
        /// </summary>
        /// <param name="personalEventQueryable">IQueryable of UserPersonalEvent.</param>
        /// <returns>.</returns>
        public static IQueryable<UserPersonalEvent> GetAcceptedEvents(
            this IQueryable<UserPersonalEvent> personalEventQueryable)
        {
            return personalEventQueryable.Where(upe => upe.IsAccepted);
        }
    }
}
