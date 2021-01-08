using System.Threading;
using System.Threading.Tasks;
using Microservice.Badge.Application.Models;
using Microservice.Badge.Infrastructure;
using Microservice.Badge.Infrastructure.Extensions;
using MongoDB.Driver;
using Thunder.Platform.Core.Application.Dtos;
using Thunder.Platform.Core.Context;

namespace Microservice.Badge.Application.Queries
{
    public class GetCommunityUserBadgesByCommunityIdQueryHandler : BaseQueryHandler<GetCommunityUserBadgesByCommunityIdQuery, PagedResultDto<UserBadgeModel>>
    {
        public GetCommunityUserBadgesByCommunityIdQueryHandler(IUserContext userContext, BadgeDbContext dbContext) : base(userContext, dbContext)
        {
        }

        protected override Task<PagedResultDto<UserBadgeModel>> HandleAsync(GetCommunityUserBadgesByCommunityIdQuery query, CancellationToken cancellationToken)
        {
            var userRewardBadges = DbContext.UserRewardCollection
                .Aggregate()
                .Match(x => x.UserId == CurrentUserId && x.CommunityId == query.CommunityId)
                .Select(p => new UserBadgeModel
                {
                    BadgeId = p.BadgeId,
                    BadgeLevel = p.BadgeLevel,
                    IssuedBy = p.IssuedBy,
                    IssuedDate = p.IssuedDate
                });

            return ApplyMongoPaging(userRewardBadges, query.PageInfo, cancellationToken);
        }
    }
}
