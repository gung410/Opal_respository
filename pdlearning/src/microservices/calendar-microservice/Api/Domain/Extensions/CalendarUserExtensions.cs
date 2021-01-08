using System;
using System.Linq;
using Microservice.Calendar.Domain.Entities;
using Microservice.Calendar.Domain.Enums;

namespace Microservice.Calendar.Domain.Extensions
{
    public static class CalendarUserExtensions
    {
        /// <summary>
        /// Filters to get learners of a Primary Approval Officer (PAO) or Alternative Approval Officer (AAO).
        /// </summary>
        /// <param name="calendarUsers">CalendarUser queryable.</param>
        /// <param name="paoOrAaoId">Primary Approval Officer (PAO) or Alternative Approval Officer ID.</param>
        /// <returns>Filtered learners queryable of PAO or AAO.</returns>
        public static IQueryable<CalendarUser> BelongsToApprovalOfficer(this IQueryable<CalendarUser> calendarUsers, Guid paoOrAaoId)
        {
            return calendarUsers.Where(u => u.PrimaryApprovalOfficerId == paoOrAaoId || u.AlternativeApprovalOfficerId == paoOrAaoId);
        }

        /// <summary>
        /// Filters to get users by status.
        /// </summary>
        /// <param name="calendarUsers">CalendarUser queryable.</param>
        /// <param name="status">Status user.</param>
        /// <returns>Filtered to get users bye status.</returns>
        public static IQueryable<CalendarUser> ByStatus(this IQueryable<CalendarUser> calendarUsers, UserStatus status)
        {
            return calendarUsers.Where(x => x.Status == Convert.ToInt32(status));
        }
    }
}
