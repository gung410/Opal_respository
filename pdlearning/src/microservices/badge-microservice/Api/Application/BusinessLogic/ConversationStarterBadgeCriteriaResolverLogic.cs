using System;
using System.Linq;
using Microservice.Badge.Application.AggregatedModels;
using Microservice.Badge.Application.Models;
using Microservice.Badge.Domain.ValueObjects;
using Microservice.Badge.Infrastructure;
using MongoDB.Driver;

namespace Microservice.Badge.Application.BusinessLogic
{
    public class ConversationStarterBadgeCriteriaResolverLogic : BaseCommunityBadgeCriteriaResolverLogic<ConversationStarterBadgeCriteria>
    {
        public ConversationStarterBadgeCriteriaResolverLogic(
            BadgeDbContext dbContext,
            IGetBadgeCriteriaLogic<ConversationStarterBadgeCriteria> getBadgeCriteriaLogic) : base(dbContext, getBadgeCriteriaLogic)
        {
        }

        public override IAggregateFluent<CommunityStatisticAggregateModel> ResolveCriteriaAsync(
            ConversationStarterBadgeCriteria badgeCriteria,
            DateTime statisticDatetime)
        {
            var result = BadgeDbContext
                .PostStatisticCollection
                .Aggregate()
                .Match(PostStatistic.IsInCommunity())
                .Match(PostStatistic.IsQualifiedConversationStarter(badgeCriteria))
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
                    UserStatistics = p.UserStatistics.Where(a => a.Count > badgeCriteria.NumOfQualifiedPost)
                });

            return result;
        }
    }
}
