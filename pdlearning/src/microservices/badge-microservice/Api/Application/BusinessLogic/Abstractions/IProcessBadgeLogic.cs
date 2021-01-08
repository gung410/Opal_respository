using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace Microservice.Badge.Application.BusinessLogic
{
    public interface IProcessBadgeLogic
    {
        /// <summary>
        /// Compare user's statistics with the list of badge definitions.
        /// If the user reaches any new badges, the system will issue that ones to the user.
        /// </summary>
        /// <param name="badgeId">badgeId.</param>
        /// <param name="userIds">IMongoQueryable userIds.</param>
        /// <param name="currentUserId">currentUserId.</param>
        /// <param name="cancellationToken">cancellationToken.</param>
        /// <returns>Task.</returns>
        Task IssueBadges(Guid badgeId, IAggregateFluent<Guid> userIds, Guid currentUserId, CancellationToken cancellationToken);

        /// <summary>
        /// Awards badge by id to users.
        /// </summary>
        /// <param name="badgeId">Badge id.</param>
        /// <param name="userIds">List user ids.</param>
        /// <param name="currentUserId">Current User id.</param>
        /// <param name="cancellationToken">CancellationToken.</param>
        /// <returns>Task.</returns>
        Task IssueBadges(
            Guid badgeId,
            List<Guid> userIds,
            Guid currentUserId,
            CancellationToken cancellationToken);
    }
}
