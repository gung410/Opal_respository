using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microservice.Badge.Application.Models;
using Microservice.Badge.Infrastructure;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Thunder.Platform.Core.Context;

namespace Microservice.Badge.Application.Queries
{
    public class GetBadgesQueryHandler : BaseQueryHandler<GetBadgesQuery, List<BadgeModel>>
    {
        public GetBadgesQueryHandler(IUserContext userContext, BadgeDbContext dbContext) : base(userContext, dbContext)
        {
        }

        protected override Task<List<BadgeModel>> HandleAsync(GetBadgesQuery query, CancellationToken cancellationToken)
        {
            return DbContext.BadgeCollection
                .AsQueryable()
                .Select(badge => new BadgeModel
                {
                    Id = badge.Id,
                    Type = badge.Type,
                    Name = badge.Name,
                    LevelImages = badge.LevelImages,
                    TagImage = badge.TagImage
                })
                .ToListAsync(cancellationToken);
        }
    }
}
