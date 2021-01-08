using System;
using System.Linq;
using Microservice.Badge.Application.AggregatedModels;
using Microservice.Badge.Application.Models;
using Microservice.Badge.Domain.ValueObjects;
using Microservice.Badge.Infrastructure;
using MongoDB.Driver;

namespace Microservice.Badge.Application.BusinessLogic
{
    public class ConversationBoosterBadgeCriteriaResolverLogic : BaseCommunityBadgeCriteriaResolverLogic<ConversationBoosterBadgeCriteria>
    {
        public ConversationBoosterBadgeCriteriaResolverLogic(BadgeDbContext dbContext, IGetBadgeCriteriaLogic<ConversationBoosterBadgeCriteria> getBadgeCriteriaLogic) : base(dbContext, getBadgeCriteriaLogic)
        {
        }

        public override IAggregateFluent<CommunityStatisticAggregateModel> ResolveCriteriaAsync(ConversationBoosterBadgeCriteria badgeCriteria, DateTime statisticDatetime)
        {
            var result = BadgeDbContext.CommunityYearlyUserStatisticCollection
                .Aggregate()
                .Match(p => p.LatestMonthlyStatistics.NumOfCreatedForum > 0)
                .SortByDescending(p => p.LatestMonthlyStatistics.NumOfCreatedForum)
                .Group(p => new { p.CommunityId }, g => new
                {
                    g.Key,
                    UserStatistics = g.Select(a => new UserStatistic
                    {
                        UserId = a.UserId,
                        Count = a.LatestMonthlyStatistics.NumOfCreatedForum
                    })
                })
                .Project(p => new CommunityStatisticAggregateModel
                {
                    CommunityId = p.Key.CommunityId,
                    UserStatistics = p.UserStatistics
                });

            return result;
        }
    }
}
