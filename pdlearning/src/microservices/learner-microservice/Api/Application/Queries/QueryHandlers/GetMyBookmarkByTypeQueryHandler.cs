using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microservice.Learner.Application.Models;
using Microservice.Learner.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Application.Dtos;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Repositories;

namespace Microservice.Learner.Application.Queries.QueryHandlers
{
    public class GetMyBookmarkByTypeQueryHandler : BaseQueryHandler<GetMyBookmarkByTypeQuery, PagedResultDto<UserBookmarkModel>>
    {
        private readonly IRepository<UserBookmark> _userBookmarkRepository;

        public GetMyBookmarkByTypeQueryHandler(
            IRepository<UserBookmark> userBookmarkRepository,
            IUserContext userContext) : base(userContext)
        {
            _userBookmarkRepository = userBookmarkRepository;
        }

        protected override async Task<PagedResultDto<UserBookmarkModel>> HandleAsync(GetMyBookmarkByTypeQuery query, CancellationToken cancellationToken)
        {
            var userBookmarksQuery = _userBookmarkRepository
                .GetAll()
                .Where(p => p.UserId == CurrentUserId)
                .Where(p => p.ItemType == query.ItemType);

            var totalCount = await userBookmarksQuery.CountAsync(cancellationToken);

            var sortCriteria = $"{nameof(UserBookmark.CreatedDate)} DESC";
            userBookmarksQuery = ApplySorting(userBookmarksQuery, query.PageInfo, sortCriteria);
            userBookmarksQuery = ApplyPaging(userBookmarksQuery, query.PageInfo);

            var userBookmarks = await userBookmarksQuery.ToListAsync(cancellationToken);

            return new PagedResultDto<UserBookmarkModel>(totalCount, userBookmarks.Select(p => new UserBookmarkModel(p)).ToList());
        }
    }
}
