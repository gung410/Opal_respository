using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microservice.Calendar.Application.Models;
using Microservice.Calendar.Domain.Entities;
using Microservice.Calendar.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Domain.Repositories;

namespace Microservice.Calendar.Domain.Extensions
{
    public static class TeamAccessSharingExtensions
    {
        /// <summary>
        /// Get shared users with check on the Users table.
        /// </summary>
        /// <param name="accessRepository"><see cref="TeamAccessSharing"/> repository.</param>
        /// <param name="userIds">The IDs of users.</param>
        /// <param name="ownerId">The ID of person who shared the access.</param>
        /// <param name="userRepo">The <see cref="CalendarUser"/> repository.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/>.</param>
        /// <returns>The sharing accesses with the users.</returns>
        public static Task<List<TeamAccessSharing>> GetAccessSharingsAsync(
            this IRepository<TeamAccessSharing> accessRepository,
            IEnumerable<Guid> userIds,
            Guid ownerId,
            IRepository<CalendarUser> userRepo,
            CancellationToken cancellationToken = default)
        {
            return (from access in accessRepository.GetAll().Where(s => s.OwnerId == ownerId && userIds.Contains(s.UserId))
                    join user in userRepo.GetAll().BelongsToApprovalOfficer(ownerId)
                        on access.UserId equals user.Id
                    select access)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Get access sharings of users by an Approval Officer.
        /// </summary>
        /// <param name="teamAccessSharingRepo"><see cref="TeamAccessSharing"/> repository.</param>
        /// <param name="ownerId">An Approval Officer ID.</param>
        /// <param name="userRepo"><see cref="CalendarUser"/> repository.</param>
        /// <returns>The access sharings of users.</returns>
        public static IQueryable<UserAccessSharingModel> GetUserAccessSharings(
            this IRepository<TeamAccessSharing> teamAccessSharingRepo,
            Guid ownerId,
            IRepository<CalendarUser> userRepo)
        {
            var usersQuery = userRepo.GetAll().ByStatus(UserStatus.Active).BelongsToApprovalOfficer(ownerId);
            var accessesQuery = teamAccessSharingRepo.GetAll().Where(a => a.OwnerId == ownerId);

            return from user in usersQuery
                   join access in accessesQuery
                       on user.Id equals access.UserId into accessGroup
                   from subAccess in accessGroup.DefaultIfEmpty()
                   select new UserAccessSharingModel
                   {
                       UserId = user.Id,
                       Email = user.Email,
                       FullName = user.FullName(),
                       Shared = subAccess != null
                   };
        }

        /// <summary>
        /// Gets shared teams of a user.
        /// </summary>
        /// <param name="teamAccessSharingRepo"><see cref="TeamAccessSharing"/> repository.</param>
        /// <param name="userId">User's ID.</param>
        /// <param name="userRepo"><see cref="CalendarUser"/> repository.</param>
        /// <returns>Teams are shared with the user.</returns>
        public static IQueryable<SharedTeamModel> GetSharedTeams(
            this IRepository<TeamAccessSharing> teamAccessSharingRepo,
            Guid userId,
            IRepository<CalendarUser> userRepo)
        {
            var accessesQuery = teamAccessSharingRepo.GetAll().Where(a => a.UserId == userId);

            return from access in accessesQuery
                   join user in userRepo.GetAll()
                      on access.OwnerId equals user.Id
                   select new SharedTeamModel
                   {
                       AccessShareId = access.Id,
                       OwnerFullName = (user.FirstName + " " + user.LastName).Trim()
                   };
        }
    }
}
