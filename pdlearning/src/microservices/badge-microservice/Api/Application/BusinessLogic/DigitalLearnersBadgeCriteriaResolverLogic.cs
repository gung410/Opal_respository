using System;
using Microservice.Badge.Application.Models;
using Microservice.Badge.Domain.Entities;
using Microservice.Badge.Domain.ValueObjects;
using Microservice.Badge.Infrastructure;
using MongoDB.Driver;

namespace Microservice.Badge.Application.BusinessLogic
{
    public class DigitalLearnersBadgeCriteriaResolverLogic : BaseGeneralBadgeCriteriaResolverLogic<DigitalLearnersBadgeCriteria, YearlyUserStatistic>
    {
        public DigitalLearnersBadgeCriteriaResolverLogic(
            BadgeDbContext dbContext,
            IGetBadgeCriteriaLogic<DigitalLearnersBadgeCriteria> getBadgeCriteriaLogic) : base(dbContext, getBadgeCriteriaLogic)
        {
        }

        public override IAggregateFluent<YearlyUserStatistic> ResolveCriteriaAsync(DigitalLearnersBadgeCriteria badgeCriteria, DateTime statisticDatetime)
        {
            return BadgeDbContext.YearlyUserStatisticCollection.Aggregate()
                .Match(p => p.Year == statisticDatetime.Year)
                .Match(YearlyUserStatistic.MatchedDigitalLearnersBadgeCriteria(badgeCriteria))
                .SortByDescending(x => x.LatestMonthlyStatistics.Total);
        }
    }
}
