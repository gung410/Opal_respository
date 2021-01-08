using System;
using Microservice.Badge.Application.Models;
using Microservice.Badge.Domain.Entities;
using Microservice.Badge.Domain.ValueObjects;
using Microservice.Badge.Infrastructure;
using MongoDB.Driver;

namespace Microservice.Badge.Application.BusinessLogic
{
    public class CollaborativeLearnersBadgeCriteriaResolverLogic : BaseGeneralBadgeCriteriaResolverLogic<CollaborativeLearnersBadgeCriteria, YearlyUserStatistic>
    {
        public CollaborativeLearnersBadgeCriteriaResolverLogic(
            BadgeDbContext dbContext,
            IGetBadgeCriteriaLogic<CollaborativeLearnersBadgeCriteria> getBadgeCriteriaLogic) : base(dbContext, getBadgeCriteriaLogic)
        {
        }

        public override IAggregateFluent<YearlyUserStatistic> ResolveCriteriaAsync(CollaborativeLearnersBadgeCriteria badgeCriteria, DateTime statisticDatetime)
        {
            return BadgeDbContext.YearlyUserStatisticCollection.Aggregate()
                .Match(p => p.Year == statisticDatetime.Year)
                .Match(YearlyUserStatistic.MatchedCollaborativeLearnersBadgeCriteria(badgeCriteria))
                .SortByDescending(p => p.LatestDailyStatistic.Total);
        }
    }
}
