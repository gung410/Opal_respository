using System;
using Microservice.Badge.Domain.Entities;
using Microservice.Badge.Domain.ValueObjects;
using Microservice.Badge.Infrastructure;
using MongoDB.Driver;

namespace Microservice.Badge.Application.BusinessLogic
{
    public class ReflectiveLearnersBadgeCriteriaResolverLogic : BaseGeneralBadgeCriteriaResolverLogic<ReflectiveLearnersBadgeCriteria, YearlyUserStatistic>
    {
        public ReflectiveLearnersBadgeCriteriaResolverLogic(
            BadgeDbContext dbContext,
            IGetBadgeCriteriaLogic<ReflectiveLearnersBadgeCriteria> badgeCriteriaStorage) : base(dbContext, badgeCriteriaStorage)
        {
        }

        public override IAggregateFluent<YearlyUserStatistic> ResolveCriteriaAsync(ReflectiveLearnersBadgeCriteria badgeCriteria, DateTime statisticDatetime)
        {
            return BadgeDbContext
            .YearlyUserStatisticCollection
            .Aggregate()
            .Match(p => p.Year == statisticDatetime.Year)
            .Match(YearlyUserStatistic.MatchedReflectiveLearnersBadgeCriteria(badgeCriteria))
            .SortByDescending(p => p.LatestDailyStatistic.ReflectiveActivityTotal);
        }
    }
}
