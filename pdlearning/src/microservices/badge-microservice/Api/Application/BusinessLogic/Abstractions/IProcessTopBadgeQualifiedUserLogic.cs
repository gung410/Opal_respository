using System;
using System.Threading.Tasks;
using Microservice.Badge.Domain.Entities;
using MongoDB.Driver;

namespace Microservice.Badge.Application.BusinessLogic
{
    public interface IProcessTopBadgeQualifiedUserLogic
    {
        /// <summary>
        /// Insert qualified users to TopBadgeQualifiedUser collection with general statistic.
        /// </summary>
        /// <param name="badgeId">badgeId which list user are qualified for.</param>
        /// <param name="qualifiedUserStatistic">list qualified users.</param>
        /// <returns>Task.</returns>
        public Task ExecuteAsync(Guid badgeId, IAggregateFluent<YearlyUserStatistic> qualifiedUserStatistic);

        /// <summary>
        /// Insert qualified users to TopBadgeQualifiedUser collection with general statistic.
        /// </summary>
        /// <param name="badgeId">badgeId which list user are qualified for.</param>
        /// <param name="qualifiedUserIds">list qualified users.</param>
        /// <returns>Task.</returns>
        public Task ExecuteAsync(Guid badgeId, IAggregateFluent<Guid> qualifiedUserIds);
    }
}
