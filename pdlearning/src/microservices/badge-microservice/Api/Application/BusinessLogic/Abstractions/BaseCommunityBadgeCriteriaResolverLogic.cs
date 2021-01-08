using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microservice.Badge.Application.AggregatedModels;
using Microservice.Badge.Domain.Enums;
using Microservice.Badge.Domain.ValueObjects;
using Microservice.Badge.Infrastructure;
using Microservice.Badge.Infrastructure.Extensions;
using MongoDB.Driver;

namespace Microservice.Badge.Application.BusinessLogic
{
    public abstract class BaseCommunityBadgeCriteriaResolverLogic<TCriteria> :
        BaseBadgeCriteriaResolverLogic<TCriteria>,
        ICommunityBuilderBadgeCriteriaResolverLogic<TCriteria>
        where TCriteria : BaseBadgeCriteria
    {
        protected BaseCommunityBadgeCriteriaResolverLogic(
            BadgeDbContext dbContext,
            IGetBadgeCriteriaLogic<TCriteria> getBadgeCriteriaLogic) : base(dbContext, getBadgeCriteriaLogic)
        {
        }

        public async Task<List<IAggregateFluent<CommunityStatisticAggregateModel>>> GetQualifiedUserAsync(DateTime statisticDatetime)
        {
            var badgeCriteria = await GetBadgeCriteriaLogic.ExecuteAsync();
            var usersCanWinBadge = ResolveCriteriaAsync(badgeCriteria, statisticDatetime);
            var qualifiedUsers = await ResolveLimitAsync(usersCanWinBadge, badgeCriteria.Limitation);
            return qualifiedUsers;
        }

        public abstract IAggregateFluent<CommunityStatisticAggregateModel> ResolveCriteriaAsync(
            TCriteria badgeCriteria,
            DateTime statisticDatetime);

        public async Task<List<IAggregateFluent<CommunityStatisticAggregateModel>>> ResolveLimitAsync(
            IAggregateFluent<CommunityStatisticAggregateModel> summaryCommunityBuilderAggregate,
            RewardBadgeLimitation rewardBadgeLimitation)
        {
            var communities = await summaryCommunityBuilderAggregate
                .Select(x => x.CommunityId)
                .ToListAsync();
            var topCommunityBuilderByCommunities = new List<IAggregateFluent<CommunityStatisticAggregateModel>>();
            foreach (var communityId in communities)
            {
                var currentSummaryCommunityBuilderAggregate = summaryCommunityBuilderAggregate
                    .Match(x => x.CommunityId == communityId);
                currentSummaryCommunityBuilderAggregate = await AddLimitation(rewardBadgeLimitation, currentSummaryCommunityBuilderAggregate);

                topCommunityBuilderByCommunities.Add(currentSummaryCommunityBuilderAggregate);
            }

            return topCommunityBuilderByCommunities;
        }

        private async Task<IAggregateFluent<CommunityStatisticAggregateModel>> AddLimitation(RewardBadgeLimitation rewardBadgeLimitation, IAggregateFluent<CommunityStatisticAggregateModel> currentSummaryCommunityBuilderAggregate)
        {
            var result = currentSummaryCommunityBuilderAggregate;
            switch (rewardBadgeLimitation.LimitType)
            {
                case RewardBadgeLimitType.MaximumPeople:
                    result = currentSummaryCommunityBuilderAggregate.Limit(rewardBadgeLimitation.LimitValues[rewardBadgeLimitation.LimitType]);
                    break;
                case RewardBadgeLimitType.TopPercent:
                    {
                        var totalUserCanWinBadge = await currentSummaryCommunityBuilderAggregate.CountAsync();
                        var numberOfUserLimit = totalUserCanWinBadge *
                                                (rewardBadgeLimitation.LimitValues[rewardBadgeLimitation.LimitType] / 100);

                        // In case the top percent of user equals zero then all users can be issued the badge.
                        if (numberOfUserLimit == 0)
                        {
                            numberOfUserLimit = totalUserCanWinBadge;
                        }

                        result = numberOfUserLimit > 0
                            ? currentSummaryCommunityBuilderAggregate.Limit(numberOfUserLimit)
                            : currentSummaryCommunityBuilderAggregate;
                    }

                    break;

                case RewardBadgeLimitType.MinOfEitherTopPercentOrMaximumPeople:
                    {
                        var maxUserCanWinBadge = await currentSummaryCommunityBuilderAggregate.CountAsync();
                        var numberOfUserLimitByPercent = maxUserCanWinBadge *
                                                         (rewardBadgeLimitation.LimitValues[RewardBadgeLimitType.TopPercent] / 100);
                        var numberOfUserLimitByMaximum =
                            rewardBadgeLimitation.LimitValues[RewardBadgeLimitType.MaximumPeople];

                        var numberOfUserLimit = Math.Min(numberOfUserLimitByPercent, numberOfUserLimitByMaximum);
                        result = numberOfUserLimit > 0
                            ? currentSummaryCommunityBuilderAggregate.Limit(numberOfUserLimit)
                            : currentSummaryCommunityBuilderAggregate;
                    }

                    break;
                default:
                    break;
            }

            return result;
        }
    }
}
