using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microservice.Learner.Application.Models;
using Microservice.Learner.Application.SharedQueries.Abstractions;
using Microservice.Learner.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Application.Dtos;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Repositories;

namespace Microservice.Learner.Application.Queries.QueryHandlers
{
    public class GetUserBookmarksQueryHandler : BaseQueryHandler<GetUserBookmarksQuery, PagedResultDto<CourseModel>>
    {
        private readonly IReadMyCourseShared _readMyCourseShared;
        private readonly IReadOnlyRepository<UserBookmark> _readUserBookmarkRepository;

        public GetUserBookmarksQueryHandler(
            IUserContext userContext,
            IReadMyCourseShared readMyCourseShared,
            IReadOnlyRepository<UserBookmark> readUserBookmarkRepository) : base(userContext)
        {
            _readMyCourseShared = readMyCourseShared;
            _readUserBookmarkRepository = readUserBookmarkRepository;
        }

        protected override async Task<PagedResultDto<CourseModel>> HandleAsync(GetUserBookmarksQuery query, CancellationToken cancellationToken)
        {
            var userBookmarkQuery = _readUserBookmarkRepository
                .GetAll()
                .Where(p => p.UserId == CurrentUserId);

            if (query.BookmarkTypeFilter?.Count > 0)
            {
                userBookmarkQuery = userBookmarkQuery.Where(p => query.BookmarkTypeFilter.Contains(p.ItemType));
            }

            var totalCount = await userBookmarkQuery.CountAsync(cancellationToken);

            var sortCriteria = $"{nameof(UserBookmark.CreatedDate)} DESC";
            userBookmarkQuery = ApplySorting(userBookmarkQuery, query.PageInfo, sortCriteria);
            userBookmarkQuery = ApplyPaging(userBookmarkQuery, query.PageInfo);

            var userBookmarks = await userBookmarkQuery.ToListAsync(cancellationToken);

            var userBookmarkItemIds = userBookmarks.Select(p => p.ItemId).ToList();

            var myCourses = await _readMyCourseShared
                .GetByUserIdAndCourseIds(CurrentUserIdOrDefault, userBookmarkItemIds);

            var items = await _readMyCourseShared
                .GetRelatedInfoOfCourses(
                    CurrentUserIdOrDefault,
                    userBookmarkItemIds,
                    userBookmarks,
                    myCourses);

            return new PagedResultDto<CourseModel>(totalCount, items);
        }
    }
}
