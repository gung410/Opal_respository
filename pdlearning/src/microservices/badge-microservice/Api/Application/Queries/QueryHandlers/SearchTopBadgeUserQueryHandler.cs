using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microservice.Badge.Application.Models;
using Microservice.Badge.Domain.Constants;
using Microservice.Badge.Infrastructure;
using Microservice.Badge.Infrastructure.Extensions;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Thunder.Platform.Core.Application.Dtos;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Extensions;

namespace Microservice.Badge.Application.Queries
{
    public class SearchTopBadgeUserQueryHandler : BaseQueryHandler<SearchTopBadgeUserQuery, PagedResultDto<UserRewardStatisticModel>>
    {
        public SearchTopBadgeUserQueryHandler(
            IUserContext userContext,
            BadgeDbContext dbContext) : base(userContext, dbContext)
        {
        }

        protected override async Task<PagedResultDto<UserRewardStatisticModel>> HandleAsync(SearchTopBadgeUserQuery query, CancellationToken cancellationToken)
        {
            Dictionary<Guid, YearlyUserStatisticType> statisticType = new Dictionary<Guid, YearlyUserStatisticType>
            {
                { BadgeIdsConstants._collaborativeLearnersBadgeId, YearlyUserStatisticType.LatestMonthly },
                { BadgeIdsConstants._reflectiveLearnersBadgeId, YearlyUserStatisticType.LatestMonthly },
                { BadgeIdsConstants._activeContributorBadgeId, YearlyUserStatisticType.LatestMonthly },
                { BadgeIdsConstants._digitalLearnersBadgeId, YearlyUserStatisticType.LatestMonthly },
                { BadgeIdsConstants._lifeLongBadgeId, YearlyUserStatisticType.Yearly },
                { BadgeIdsConstants._conversationStarterBadgeId, YearlyUserStatisticType.LatestMonthly },
                { BadgeIdsConstants._conversationBoosterBadgeId, YearlyUserStatisticType.LatestMonthly },
                { BadgeIdsConstants._visualStorytellerBadgeId, YearlyUserStatisticType.LatestMonthly },
                { BadgeIdsConstants._linkCuratorBadgeId, YearlyUserStatisticType.LatestMonthly }
            };
            var users = await DbContext
                .TopBadgeQualifiedUserCollection
                .FindFullTextIndex(query.SearchText)
                .Where(p => p.BadgeId == query.BadgeId)
                .Paging(query.PageInfo)
                .ToListAsync(cancellationToken);

            var userIds = users.Select(a => a.Id);
            var userRewards = await DbContext.UserRewardCollection
                .AsQueryableWhere(p => userIds.Contains(p.UserId))
                .ToListAsync(cancellationToken);

            var userRewardDict = userRewards.GroupBy(p => p.UserId).ToDictionary(p => p.Key, p => p.ToList());

            return new PagedResultDto<UserRewardStatisticModel>(
                users.Count,
                users.SelectList(p =>
                {
                    var awardedBadges = userRewardDict.GetValueOrDefault(p.Id) ?? new();
                    return new UserRewardStatisticModel
                    {
                        Awarded = awardedBadges.Any(a => a.BadgeId == query.BadgeId),
                        UserId = p.UserId,
                        AwardedBadges = awardedBadges.SelectList(awardedBadge => new UserBadgeModel
                        {
                            BadgeId = awardedBadge.BadgeId,
                            BadgeLevel = awardedBadge.BadgeLevel,
                            IssuedBy = awardedBadge.IssuedBy,
                            IssuedDate = awardedBadge.IssuedDate,
                            CommunityId = awardedBadge.CommunityId
                        }),
                        Type = statisticType.GetValueOrDefault(p.BadgeId),
                        Statistic = new UserStatisticModel(p.CreatedDate.Year, p.GeneralStatistic)
                    };
                }));
        }
    }
}
