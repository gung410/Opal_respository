using System.Threading;
using System.Threading.Tasks;
using Microservice.Badge.Application.Models;
using Microservice.Badge.Domain.Constants;
using Microservice.Badge.Domain.ValueObjects;
using Microservice.Badge.Infrastructure;
using Microservice.Badge.Infrastructure.Extensions;
using MongoDB.Driver.Linq;
using Thunder.Platform.Core.Context;

namespace Microservice.Badge.Application.Queries
{
    public class GetActiveContributorBadgeQueryHandler : BaseQueryHandler<GetActiveContributorBadgeQuery, BadgeWithCriteriaModel<ActiveContributorsBadgeCriteria>>
    {
        public GetActiveContributorBadgeQueryHandler(IUserContext userContext, BadgeDbContext dbContext) : base(userContext, dbContext)
        {
        }

        protected override Task<BadgeWithCriteriaModel<ActiveContributorsBadgeCriteria>> HandleAsync(GetActiveContributorBadgeQuery query, CancellationToken cancellationToken)
        {
            return DbContext.GetBadgeCriteriaCollection<ActiveContributorsBadgeCriteria>()
                .AsQueryableWhere(p => p.Id == BadgeIdsConstants._activeContributorBadgeId)
                .Select(p => new BadgeWithCriteriaModel<ActiveContributorsBadgeCriteria>
                {
                    Id = p.Id,
                    Type = p.Type,
                    Name = p.Name,
                    TagImage = p.TagImage,
                    LevelImages = p.LevelImages,
                    Criteria = p.Criteria
                })
                .FirstOrDefaultAsync(cancellationToken);
        }
    }
}
