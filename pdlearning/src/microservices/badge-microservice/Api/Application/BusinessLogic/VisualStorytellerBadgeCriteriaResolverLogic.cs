using System;
using System.Linq;
using Microservice.Badge.Application.AggregatedModels;
using Microservice.Badge.Domain.ValueObjects;
using Microservice.Badge.Infrastructure;
using MongoDB.Driver;

namespace Microservice.Badge.Application.BusinessLogic
{
    public class VisualStorytellerBadgeCriteriaResolverLogic : BaseCommunityBadgeCriteriaResolverLogic<VisualStorytellerBadgeCriteria>
    {
        public VisualStorytellerBadgeCriteriaResolverLogic(
            BadgeDbContext dbContext,
            IGetBadgeCriteriaLogic<VisualStorytellerBadgeCriteria> linkCuratorCriteriaStorage) : base(dbContext, linkCuratorCriteriaStorage)
        {
        }

        public override IAggregateFluent<CommunityStatisticAggregateModel> ResolveCriteriaAsync(VisualStorytellerBadgeCriteria badgeCriteria, DateTime statisticDatetime)
        {
            var result = BadgeDbContext
            .PostStatisticCollection
            .Aggregate()
            .Match(PostStatistic.IsInCommunity())
            .Match(PostStatistic.IsQualifiedVisualStoryteller(badgeCriteria))
            .Group(x => new { x.CreatedBy, x.CommunityId }, g => new
            {
                g.Key,
                Count = g.Sum(a => 1),
            })
            .SortByDescending(p => p.Count)
            .Group(x => new { x.Key.CommunityId }, g => new
            {
                g.Key,
                UserStatistics = g.Select(p => new UserStatistic
                {
                    UserId = p.Key.CreatedBy,
                    Count = p.Count
                })
            })
            .Project(p => new CommunityStatisticAggregateModel
            {
                CommunityId = (Guid)p.Key.CommunityId,
                UserStatistics = p.UserStatistics.Where(a => a.Count >= badgeCriteria.NumOfQualifiedVisualPost)
            });

            return result;
        }
    }
}
