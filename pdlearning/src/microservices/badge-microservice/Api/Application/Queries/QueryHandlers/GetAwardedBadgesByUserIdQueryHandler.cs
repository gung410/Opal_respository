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
    public class GetAwardedBadgesByUserIdQueryHandler : BaseQueryHandler<GetAwardedBadgesByUserIdQuery, PagedResultDto<UserBadgeModel>>
    {
        public GetAwardedBadgesByUserIdQueryHandler(IUserContext userContext, BadgeDbContext dbContext) : base(
            userContext, dbContext)
        {
        }

        protected override Task<PagedResultDto<UserBadgeModel>> HandleAsync(
            GetAwardedBadgesByUserIdQuery query,
            CancellationToken cancellationToken)
        {
            var generalUserRewardQuery = DbContext.UserRewardCollection
                .Aggregate()
                .Match(p => p.UserId == query.UserId && p.CommunityId == null)
                .SortBy(p => p.IssuedDate)
                .Select(p => new UserBadgeModel
                {
                    BadgeId = p.BadgeId,
                    BadgeLevel = p.BadgeLevel,
                    IssuedBy = p.IssuedBy,
                    IssuedDate = p.IssuedDate
                });

            var communityUserRewardQuery = DbContext.UserRewardCollection
                .Aggregate()
                .Match(p => p.UserId == query.UserId && p.CommunityId != null)
                .SortBy(p => p.IssuedDate)
                .Select(p => new UserBadgeModel
                {
                    BadgeId = p.BadgeId,
                    CommunityId = p.CommunityId,
                    BadgeLevel = p.BadgeLevel,
                    IssuedBy = p.IssuedBy,
                    IssuedDate = p.IssuedDate
                });

            return ApplyMongoPagingQueries(generalUserRewardQuery, communityUserRewardQuery, query.PageInfo, cancellationToken);
        }
    }
}
