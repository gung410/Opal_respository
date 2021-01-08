using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microservice.Badge.Application.Models;
using Microservice.Badge.Infrastructure;
using Microservice.Badge.Infrastructure.Extensions;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Thunder.Platform.Core.Context;

namespace Microservice.Badge.Application.Queries
{
    public class GetGeneralBadgesByUserIdQueryHandler : BaseQueryHandler<GetGeneralBadgesByUserIdQuery, List<UserBadgeModel>>
    {
        public GetGeneralBadgesByUserIdQueryHandler(IUserContext userContext, BadgeDbContext dbContext) : base(userContext, dbContext)
        {
        }

        protected override Task<List<UserBadgeModel>> HandleAsync(GetGeneralBadgesByUserIdQuery query, CancellationToken cancellationToken)
        {
            return DbContext.UserRewardCollection
                .AsQueryableWhere(p => p.UserId == query.UserId)
                .Select(p => new UserBadgeModel
                {
                    BadgeId = p.BadgeId,
                    BadgeLevel = p.BadgeLevel,
                    IssuedBy = p.IssuedBy,
                    IssuedDate = p.IssuedDate
                })
                .ToListAsync(cancellationToken);
        }
    }
}
