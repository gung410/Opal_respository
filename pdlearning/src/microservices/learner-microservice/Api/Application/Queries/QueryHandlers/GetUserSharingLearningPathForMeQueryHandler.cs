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
    public class GetUserSharingLearningPathForMeQueryHandler : BaseQueryHandler<GetUserSharingLearningPathForMeQuery, PagedResultDto<LearnerLearningPathModel>>
    {
        private readonly IRepository<UserSharing> _userSharingRepository;
        private readonly IRepository<UserSharingDetail> _userSharingDetailRepository;
        private readonly IRepository<LearnerLearningPath> _learnerLearningPathRepository;
        private readonly IRepository<LearnerLearningPathCourse> _learnerLearningPathCourseRepository;
        private readonly IRepository<UserBookmark> _userBookmarkRepository;

        public GetUserSharingLearningPathForMeQueryHandler(
            IRepository<UserSharing> userSharingRepository,
            IRepository<UserSharingDetail> userSharingDetailRepository,
            IRepository<LearnerLearningPath> learnerLearningPathRepository,
            IRepository<UserBookmark> userBookmarkRepository,
            IRepository<LearnerLearningPathCourse> learnerLearningPathCourseRepository,
            IUserContext userContext) : base(userContext)
        {
            _userSharingRepository = userSharingRepository;
            _userSharingDetailRepository = userSharingDetailRepository;
            _learnerLearningPathRepository = learnerLearningPathRepository;
            _learnerLearningPathCourseRepository = learnerLearningPathCourseRepository;
            _userBookmarkRepository = userBookmarkRepository;
        }

        protected override async Task<PagedResultDto<LearnerLearningPathModel>> HandleAsync(GetUserSharingLearningPathForMeQuery query, CancellationToken cancellationToken)
        {
            var userSharedForMe = await _userSharingDetailRepository
                .GetAll()
                .Where(p => p.UserId == CurrentUserId)
                .Select(p => p.UserSharingId)
                .Distinct()
                .ToListAsync(cancellationToken);

            var userSharingQuery = _userSharingRepository
                .GetAll()
                .Where(p => p.ItemType == query.ItemType)
                .Where(p => userSharedForMe.Contains(p.Id));

            var totalCount = await userSharingQuery.CountAsync(cancellationToken);

            var sortCriteria = $"{nameof(UserSharing.CreatedDate)} DESC";
            userSharingQuery = ApplySorting(userSharingQuery, query.PageInfo, sortCriteria);
            userSharingQuery = ApplyPaging(userSharingQuery, query.PageInfo);

            var userSharingItemIds = await userSharingQuery.Select(p => p.ItemId).ToListAsync(cancellationToken);

            var learningPaths = await _learnerLearningPathRepository
                .GetAll()
                .Where(p => userSharingItemIds.Contains(p.Id))
                .ToListAsync(cancellationToken);

            var learningPathIds = learningPaths.Select(p => p.Id).ToList();

            var bookmarks = await _userBookmarkRepository
                .GetAll()
                .Where(p => p.UserId == CurrentUserId)
                .Where(p => learningPathIds.Contains(p.ItemId))
                .ToListAsync(cancellationToken);

            var learningPathCourses = await _learnerLearningPathCourseRepository
                .GetAll()
                .Where(p => learningPathIds.Contains(p.LearningPathId))
                .ToListAsync(cancellationToken);

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

            return new PagedResultDto<LearnerLearningPathModel>(totalCount, learningPathItems);
        }
    }
}
