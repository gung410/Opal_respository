using System;
using System.Threading.Tasks;
using Microservice.Badge.Domain.Enums;
using Microservice.Badge.Domain.ValueObjects;
using Microservice.Badge.Infrastructure;
using Microservice.Badge.Infrastructure.Extensions;
using MongoDB.Driver;

namespace Microservice.Badge.Application.BusinessLogic
{
    public abstract class BaseGeneralBadgeCriteriaResolverLogic<TCriteria, TResult> :
        BaseBadgeCriteriaResolverLogic<TCriteria>,
        IGeneralBadgeCriteriaResolverLogic<TCriteria, TResult>
        where TCriteria : BaseBadgeCriteria
    {
        protected BaseGeneralBadgeCriteriaResolverLogic(
            BadgeDbContext dbContext,
            IGetBadgeCriteriaLogic<TCriteria> getBadgeCriteriaLogic) : base(dbContext, getBadgeCriteriaLogic)
        {
        }

        public abstract IAggregateFluent<TResult> ResolveCriteriaAsync(TCriteria badgeCriteria, DateTime statisticDatetime);

        public async Task<IAggregateFluent<TResult>> GetQualifiedUserAsync(DateTime statisticDatetime)
        {
            var badgeCriteria = await GetBadgeCriteriaLogic.ExecuteAsync();
            var usersCanWinBadge = ResolveCriteriaAsync(badgeCriteria, statisticDatetime);

            if (usersCanWinBadge == null)
            {
                return null;
            }

            var qualifiedUsers = await ResolveLimitAsync(usersCanWinBadge, badgeCriteria.Limitation);
            return qualifiedUsers;
        }

        public async Task<IAggregateFluent<TResult>> ResolveLimitAsync(
            IAggregateFluent<TResult> userIdsAggregateFluent,
            RewardBadgeLimitation rewardBadgeLimitation)
        {
            switch (rewardBadgeLimitation.LimitType)
            {
                case RewardBadgeLimitType.MaximumPeople:
                    return userIdsAggregateFluent.Limit(rewardBadgeLimitation.LimitValues[rewardBadgeLimitation.LimitType]);

                case RewardBadgeLimitType.TopPercent:
                    {
                        var totalUserCanWinBadge = await userIdsAggregateFluent.CountAsync();
                        var numberOfUserLimit = totalUserCanWinBadge *
                                                (rewardBadgeLimitation.LimitValues[rewardBadgeLimitation.LimitType] / 100);

                        // In case the top percent of user equals zero then all users can be issued the badge.
                        if (numberOfUserLimit == 0)
                        {
                            numberOfUserLimit = totalUserCanWinBadge;
                        }

                        return numberOfUserLimit > 0
                            ? userIdsAggregateFluent.Limit(numberOfUserLimit)
                            : userIdsAggregateFluent;
                    }

                case RewardBadgeLimitType.MinOfEitherTopPercentOrMaximumPeople:
                    {
                        var maxUserCanWinBadge = await userIdsAggregateFluent.CountAsync();
                        var numberOfUserLimitByPercent = maxUserCanWinBadge *
                                                         (rewardBadgeLimitation.LimitValues[RewardBadgeLimitType.TopPercent] / 100);
                        var numberOfUserLimitByMaximum =
                            rewardBadgeLimitation.LimitValues[RewardBadgeLimitType.MaximumPeople];

                        var numberOfUserLimit = Math.Min(numberOfUserLimitByPercent, numberOfUserLimitByMaximum);
                        return numberOfUserLimit > 0
                            ? userIdsAggregateFluent.Limit(numberOfUserLimit)
                            : userIdsAggregateFluent;
                    }

                default:
                    return userIdsAggregateFluent;
            }
        }
    }
}
