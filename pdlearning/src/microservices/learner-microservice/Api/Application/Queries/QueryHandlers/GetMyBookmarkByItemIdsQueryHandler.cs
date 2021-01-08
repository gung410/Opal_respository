using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microservice.Learner.Application.Models;
using Microservice.Learner.Domain.Entities;
using Microservice.Learner.Domain.ValueObject;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Repositories;

namespace Microservice.Learner.Application.Queries.QueryHandlers
{
    public class GetMyBookmarkByItemIdsQueryHandler : BaseQueryHandler<GetMyBookmarkByItemIdsQuery, List<UserBookmarkModel>>
    {
        private readonly IRepository<UserBookmark> _userBookmarkRepository;

        public GetMyBookmarkByItemIdsQueryHandler(
            IRepository<UserBookmark> userBookmarkRepository,
            IUserContext userContext) : base(userContext)
        {
            _userBookmarkRepository = userBookmarkRepository;
        }

        protected override Task<List<UserBookmarkModel>> HandleAsync(GetMyBookmarkByItemIdsQuery query, CancellationToken cancellationToken)
        {
            if (query.ItemIds.Length == 0)
            {
                return Task.FromResult(new List<UserBookmarkModel>());
            }

            IQueryable<UserBookmark> userBookmarksQuery;

            // The LearningPath have 2 places: 1 at Learner and 1 at LMM => we will get bookmark for both and in the UI we call to LMM api thi to get the detail for that
            if (query.ItemType == BookmarkType.LearningPath)
            {
                userBookmarksQuery = _userBookmarkRepository
                    .GetAll()
                    .Where(p => p.UserId == CurrentUserId)
                    .Where(p => p.ItemType == query.ItemType || p.ItemType == BookmarkType.LearningPathLMM)
                    .Where(p => query.ItemIds.Contains(p.ItemId));
            }
            else
            {
                userBookmarksQuery = _userBookmarkRepository
                    .GetAll()
                    .Where(p => p.UserId == CurrentUserId)
                    .Where(p => p.ItemType == query.ItemType)
                    .Where(p => query.ItemIds.Contains(p.ItemId));
            }

            return userBookmarksQuery.Select(p => new UserBookmarkModel(p)).ToListAsync(cancellationToken);
        }
    }
}
