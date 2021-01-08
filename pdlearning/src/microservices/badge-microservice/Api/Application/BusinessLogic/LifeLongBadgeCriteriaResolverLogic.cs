using System;
using Microservice.Badge.Application.Models;
using Microservice.Badge.Domain.Constants;
using Microservice.Badge.Domain.ValueObjects;
using Microservice.Badge.Infrastructure;
using Microservice.Badge.Infrastructure.Extensions;
using MongoDB.Driver;

namespace Microservice.Badge.Application.BusinessLogic
{
    public class LifeLongBadgeCriteriaResolverLogic : BaseGeneralBadgeCriteriaResolverLogic<LifeLongBadgeCriteria, Guid>
    {
        public LifeLongBadgeCriteriaResolverLogic(
            BadgeDbContext dbContext,
            IGetBadgeCriteriaLogic<LifeLongBadgeCriteria> badgeCriteriaStorage) : base(dbContext, badgeCriteriaStorage)
        {
        }

        public override IAggregateFluent<Guid> ResolveCriteriaAsync(LifeLongBadgeCriteria badgeCriteria, DateTime statisticDatetime)
        {
            return BadgeDbContext
               .UserRewardCollection
               .Aggregate()
               .Match(p => p.BadgeId == BadgeIdsConstants._activeContributorBadgeId)
               .SortBy(p => p.IssuedDate)
               .Select(p => p.UserId);
        }
    }
}
