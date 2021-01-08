using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microservice.Learner.Application.Models;
using Microservice.Learner.Domain.Entities;
using Microservice.Learner.Domain.ValueObject;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Application.Dtos;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Repositories;

namespace Microservice.Learner.Application.Queries.QueryHandlers
{
    public class GetMyLearningPathBookmarkQueryHandler : BaseQueryHandler<GetMyLearningPathBookmarkQuery, PagedResultDto<LearnerLearningPathModel>>
    {
        private readonly IRepository<LearnerLearningPath> _learnerLearningPathRepository;
        private readonly IRepository<LearnerLearningPathCourse> _learnerLearningPathCourseRepository;
        private readonly IRepository<UserBookmark> _userBookmarkRepository;

        public GetMyLearningPathBookmarkQueryHandler(
            IRepository<LearnerLearningPath> learnerLearningPathRepository,
            IRepository<UserBookmark> userBookmarkRepository,
            IRepository<LearnerLearningPathCourse> learnerLearningPathCourseRepository,
            IUserContext userContext) : base(userContext)
        {
            _learnerLearningPathRepository = learnerLearningPathRepository;
            _learnerLearningPathCourseRepository = learnerLearningPathCourseRepository;
            _userBookmarkRepository = userBookmarkRepository;
        }

        protected override async Task<PagedResultDto<LearnerLearningPathModel>> HandleAsync(GetMyLearningPathBookmarkQuery query, CancellationToken cancellationToken)
        {
            var userBookmarks = _userBookmarkRepository
                .GetAll()
                .Where(p => p.UserId == CurrentUserId)
                .Where(p => p.ItemType == query.ItemType || p.ItemType == BookmarkType.LearningPathLMM);

            var totalCount = await userBookmarks.CountAsync(cancellationToken);

            userBookmarks = ApplySorting(userBookmarks, query.PageInfo, $"{nameof(UserBookmark.CreatedDate)} DESC");
            userBookmarks = ApplyPaging(userBookmarks, query.PageInfo);

            // Because Learner module doesn't sync learningPath from LMM
            // => we will return a list to UI and UI must be call to LMM service to get more detail information.
            var learningPathLmmItems = await userBookmarks
                .Where(p => p.ItemType == BookmarkType.LearningPathLMM)
                .Select(p => new LearnerLearningPathModel(p))
                .ToListAsync(cancellationToken);

            // 1. Get user's bookmark item ids with type == LearningPath.
            var userLearningPathItemIds = await userBookmarks
                .Where(p => p.ItemType == BookmarkType.LearningPath)
                .Select(p => p.ItemId)
                .ToListAsync(cancellationToken);

            // 2. Get user's learning paths (entity) that matched the learning path item ids above.
            var learningPaths = await _learnerLearningPathRepository
                .GetAll()
                .Where(p => userLearningPathItemIds.Contains(p.Id))
                .ToListAsync(cancellationToken);

            // 3. Get user's bookmarks that matched the learning path item ids above.
            var bookmarks = await _userBookmarkRepository
                .GetAll()
                .Where(p => p.UserId == CurrentUserId)
                .Where(p => userLearningPathItemIds.Contains(p.ItemId))
                .ToListAsync(cancellationToken);

            // 4. Get user's learning path course that matched the learning path item ids above.
            var learningPathCourses = await _learnerLearningPathCourseRepository
                .GetAll()
                .Where(p => userLearningPathItemIds.Contains(p.LearningPathId))
                .ToListAsync(cancellationToken);

            // 5. From 2,3,4 --> mapping to a list of LearnerLearningPathModel.
            var learningPathItems = learningPaths.Select(lp =>
            {
                var bookmark = bookmarks.FirstOrDefault(p => p.ItemId == lp.Id);
                var courses = learningPathCourses
                    .Where(p => p.LearningPathId == lp.Id)
                    .OrderBy(d => d.Order)
                    .ToList();

                return LearnerLearningPathModel.New(
                        lp.Id,
                        lp.Title,
                        lp.CreatedBy,
                        lp.ThumbnailUrl,
                        lp.IsPublic)
                    .WithBookmarkInfo(bookmark)
                    .WithLearningPathCourses(courses);
            }).ToList();

            // Final result.
            var learningPathResult = learningPathItems
                .Union(learningPathLmmItems)
                .OrderByDescending(p => p.BookmarkInfo.CreatedDate)
                .ToList();

            return new PagedResultDto<LearnerLearningPathModel>(totalCount, learningPathResult);
        }
    }
}
