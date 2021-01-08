using System;
using Microservice.Badge.Domain.ValueObjects;

namespace Microservice.Badge.Domain.Entities
{
    /// <summary>
    /// This entity defines activity statistic of users each year.
    /// It also contains detail statistic of user each month, each day for ensuring ability to track data for analytic.
    /// </summary>
    public class CommunityYearlyUserStatistic : BaseYearlyUserStatistic<CommunityStatistic>
    {
        public CommunityYearlyUserStatistic(Guid userId, Guid communityId, int year)
        {
            this.UserId = userId;
            this.CommunityId = communityId;
            this.Year = year;
        }

        public Guid CommunityId { get; init; }
    }
}
