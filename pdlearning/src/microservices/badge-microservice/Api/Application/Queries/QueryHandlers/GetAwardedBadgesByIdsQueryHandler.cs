using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microservice.Badge.Application.Models;
using Microservice.Badge.Domain.Entities;
using Microservice.Badge.Infrastructure;
using Microservice.Badge.Infrastructure.Extensions;
using MongoDB.Driver;
using Thunder.Platform.Core.Application.Dtos;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Extensions;

namespace Microservice.Badge.Application.Queries
{
    public class GetAwardedBadgesByIdsQueryHandler : BaseQueryHandler<GetAwardedBadgesByIdsQuery, PagedResultDto<UserBadgeModel>>
    {
        public GetAwardedBadgesByIdsQueryHandler(IUserContext userContext, BadgeDbContext dbContext) : base(
        userContext, dbContext)
        {
        }

        protected override async Task<PagedResultDto<UserBadgeModel>> HandleAsync(GetAwardedBadgesByIdsQuery query, CancellationToken cancellationToken)
        {
            var badgeIds = query.Data.SelectListDistinct(p => p.BadgeId);

            var badges = await DbContext
                .BadgeCollection
                .AsQueryableWhere(p => badgeIds.Contains(p.Id))
                .ToListAsync(cancellationToken);

            var badgesDic = badges
                .ToDictionary(p => p.Id, p => p);

            FilterDefinition<UserReward> filter = null;
            foreach (var item in query.Data)
            {
                var filterUserReward = Builders<UserReward>.Filter.Where(p =>
                    p.BadgeId == item.BadgeId && p.CommunityId == item.CommunityId);

                filter = filter != null
                    ? filter | filterUserReward
                    : filterUserReward;
            }

            var userRewards = await DbContext.UserRewardCollection
                .Aggregate()
                .Match(filter)
                .ToListAsync(cancellationToken);

            return new PagedResultDto<UserBadgeModel>(
                userRewards.Count,
                userRewards.SelectList(item => new UserBadgeModel(item, badgesDic[item.BadgeId])));
        }
    }
}
