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
    public class GetCommunityUserBadgesByUserIdQueryHandler : BaseQueryHandler<GetCommunityUserBadgesByUserIdQuery, PagedResultDto<UserBadgeModel>>
    {
        public GetCommunityUserBadgesByUserIdQueryHandler(IUserContext userContext, BadgeDbContext dbContext) : base(userContext, dbContext)
        {
        }

        protected override Task<PagedResultDto<UserBadgeModel>> HandleAsync(GetCommunityUserBadgesByUserIdQuery query, CancellationToken cancellationToken)
        {
            var communityUserRewardQuery = DbContext.UserRewardCollection
                .Aggregate()
                .Match(p => p.UserId == query.UserId && p.CommunityId != null)
                .SortBy(p => p.IssuedDate)
                .Select(p => new UserBadgeModel
                {
                    BadgeId = p.BadgeId,
                    BadgeLevel = p.BadgeLevel,
                    IssuedBy = p.IssuedBy,
                    IssuedDate = p.IssuedDate
                });

            return ApplyMongoPaging(communityUserRewardQuery, query.PageInfo, cancellationToken);
        }
    }
}
