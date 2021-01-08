using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microservice.Learner.Application.Models;
using Microservice.Learner.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Repositories;

namespace Microservice.Learner.Application.Queries.QueryHandlers
{
    public class GetCountUserBookmarkedQueryHandler : BaseQueryHandler<GetCountUserBookmarkedQuery, List<UserBookmarkedModel>>
    {
        private readonly IRepository<UserBookmark> _userBookmarkRepository;

        public GetCountUserBookmarkedQueryHandler(
            IRepository<UserBookmark> userBookmarkRepository,
            IUserContext userContext) : base(userContext)
        {
            _userBookmarkRepository = userBookmarkRepository;
        }

        protected override Task<List<UserBookmarkedModel>> HandleAsync(GetCountUserBookmarkedQuery query, CancellationToken cancellationToken)
        {
            return _userBookmarkRepository
                .GetAll()
                .Where(p => p.ItemType == query.ItemType)
                .Where(p => query.ItemIds.Contains(p.ItemId))
                .GroupBy(x => new
                {
                    x.ItemId,
                    x.ItemType
                })
                .Select(group => new UserBookmarkedModel
                {
                    ItemId = group.Key.ItemId,
                    CountTotal = group.Count()
                })
                .ToListAsync(cancellationToken);
        }
    }
}
