using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microservice.Learner.Application.Dtos;
using Microservice.Learner.Application.Exceptions;
using Microservice.Learner.Application.Models;
using Microservice.Learner.Application.SharedQueries.Abstractions;
using Microservice.Learner.Domain.Entities;
using Microservice.Learner.Domain.ValueObject;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Repositories;

namespace Microservice.Learner.Application.Queries.QueryHandlers
{
    public class SearchLearnerLearningPathQueryHandler :
        BaseQueryHandler<SearchLearnerLearningPathQuery, SearchPagedResultDto<LearnerLearningPathModel, LearningPathStatisticModel>>
    {
        private readonly IReadUserBookmarkShared _readUserBookmarkShared;
        private readonly IRepository<UserSharing> _userSharingRepository;
        private readonly IRepository<UserSharingDetail> _userSharingDetailRepository;
        private readonly IRepository<LearnerLearningPath> _learnerLearningPathRepository;
        private readonly IRepository<LearnerLearningPathCourse> _learnerLearningPathCourseRepository;

        public SearchLearnerLearningPathQueryHandler(
            IUserContext userContext,
            IRepository<UserSharing> userSharingRepository,
            IReadUserBookmarkShared readUserBookmarkShared,
            IRepository<UserSharingDetail> userSharingDetailRepository,
            IRepository<LearnerLearningPath> learnerLearningPathRepository,
            IRepository<LearnerLearningPathCourse> learnerLearningPathCourseRepository) : base(userContext)
        {
            _userSharingRepository = userSharingRepository;
            _readUserBookmarkShared = readUserBookmarkShared;
            _userSharingDetailRepository = userSharingDetailRepository;
            _learnerLearningPathRepository = learnerLearningPathRepository;
            _learnerLearningPathCourseRepository = learnerLearningPathCourseRepository;
        }

        protected override async Task<SearchPagedResultDto<LearnerLearningPathModel, LearningPathStatisticModel>> HandleAsync(
            SearchLearnerLearningPathQuery query,
            CancellationToken cancellationToken)
        {
            var learningPathQuery = _learnerLearningPathRepository
                .GetAll()
                .Where(p => p.CreatedBy == CurrentUserId);

            // Get total count of learning path items by each learning types
            var statistics =
                await CountByLearningPathTypes(query, cancellationToken);

            if (!string.IsNullOrEmpty(query.SearchText))
            {
                if (query.LearningPathType != null)
                {
                    // Filter by learning path type
                    learningPathQuery = FilerByLearningPathType(query);
                }
                else
                {
                    // When user switch to recommendation option from search engine
                    return new SearchPagedResultDto<LearnerLearningPathModel, LearningPathStatisticModel>(
                        0,
                        new List<LearnerLearningPathModel>(),
                        statistics);
                }
            }

            var totalCount = await learningPathQuery.CountAsync(cancellationToken);

            var sortCriteria = $"{nameof(LearnerLearningPath.CreatedDate)} DESC";

            learningPathQuery = ApplySorting(learningPathQuery, query.PageInfo, sortCriteria);
            learningPathQuery = ApplyPaging(learningPathQuery, query.PageInfo);

            var learningPaths = await learningPathQuery.ToListAsync(cancellationToken);

            var learningPathIds = learningPathQuery
                .Select(p => p.Id)
                .ToList();

            var bookmarks = await _readUserBookmarkShared.GetByItemIds(CurrentUserIdOrDefault, learningPathIds);

            var learningPathCourses = await _learnerLearningPathCourseRepository
                .GetAll()
                .Where(p => learningPathIds.Contains(p.LearningPathId))
                .ToListAsync(cancellationToken);

            // Map to LearnerLearningPathModel.
            var learningPathItems = learningPaths
                .Select(lp =>
                {
                    var userBookmark = bookmarks.FirstOrDefault(p => p.ItemId == lp.Id);
                    var learningPathCourse = learningPathCourses
                        .Where(p => p.LearningPathId == lp.Id)
                        .OrderBy(o => o.Order)
                        .ToList();

                    return LearnerLearningPathModel.New(
                            lp.Id,
                            lp.Title,
                            lp.CreatedBy,
                            lp.ThumbnailUrl,
                            lp.IsPublic)
                        .WithBookmarkInfo(userBookmark)
                        .WithLearningPathCourses(learningPathCourse);
                }).ToList();

            return new SearchPagedResultDto<LearnerLearningPathModel, LearningPathStatisticModel>(
                totalCount,
                learningPathItems,
                statistics);
        }

        /// <summary>
        /// Build query to filter learning path by <see cref="LearningPathType"/>.
        /// </summary>
        /// <param name="query">Request from client.</param>
        /// <returns>An <see cref="IQueryable"/>
        /// that contains the <see cref="LearnerLearningPath"/> information.</returns>
        private IQueryable<LearnerLearningPath> FilerByLearningPathType(SearchLearnerLearningPathQuery query)
        {
            switch (query.LearningPathType)
            {
                case LearningPathType.Owner:
                    return FilterByOwnerQuery(query);

                case LearningPathType.Shared:
                    return FilterBySharedToMeQuery(query);

                default:
                    throw new UnexpectedStatusException($"{query.LearningPathType}");
            }
        }

        /// <summary>
        /// Get total count of learning path items by each my learning path type.
        /// </summary>
        /// <param name="query">Request from client.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Returns <see cref="MyCourseStatisticModel"/> list
        /// to get total count of learning path items by each <see cref="LearningPathType"/>.</returns>
        private async Task<List<LearningPathStatisticModel>> CountByLearningPathTypes(
            SearchLearnerLearningPathQuery query,
            CancellationToken cancellationToken = default)
        {
            var statistics = new List<LearningPathStatisticModel>();

            if (!query.IncludeStatistic)
            {
                return statistics;
            }

            foreach (var learningPathType in query.LearningPathStatistic)
            {
                var learningPathTypeCount = await FilerByLearningPathType(query)
                    .CountAsync(cancellationToken);

                statistics.Add(new LearningPathStatisticModel(learningPathType, learningPathTypeCount));
            }

            return statistics;
        }

        /// <summary>
        /// Build query by search text condition.
        /// </summary>
        /// <param name="query">Request from client.</param>
        /// <param name="learningPathQuery">Learning path query.</param>
        /// <returns>An <see cref="IQueryable"/>
        /// that contains the <see cref="LearnerLearningPath"/> information.</returns>
        private IQueryable<LearnerLearningPath> FilterByTitleQuery(
            SearchLearnerLearningPathQuery query,
            IQueryable<LearnerLearningPath> learningPathQuery)
        {
            return learningPathQuery
                .Where(p =>
                    !string.IsNullOrEmpty(p.Title)
                    && EF.Functions.Like(p.Title, $"%{query.SearchText}%"));
        }

        /// <summary>
        /// Build query to get my own learning path.
        /// </summary>
        /// <param name="query">Request from client.</param>
        /// <returns>An <see cref="IQueryable"/>
        /// that contains the <see cref="LearnerLearningPath"/> information.</returns>
        private IQueryable<LearnerLearningPath> FilterByOwnerQuery(SearchLearnerLearningPathQuery query)
        {
            var learningPathQuery = _learnerLearningPathRepository
                .GetAll()
                .Where(p => p.CreatedBy == CurrentUserId);

            return FilterByTitleQuery(query, learningPathQuery);
        }

        /// <summary>
        /// Build query to get a shared learning path.
        /// </summary>
        /// <param name="query">Request from client.</param>
        /// <returns>An <see cref="IQueryable"/>
        /// that contains the <see cref="LearnerLearningPath"/> information.</returns>
        private IQueryable<LearnerLearningPath> FilterBySharedToMeQuery(SearchLearnerLearningPathQuery query)
        {
            var userSharingDetailQuery = _userSharingDetailRepository
                .GetAll()
                .Where(p => p.UserId == CurrentUserId);

            var userSharingQuery = _userSharingRepository
                .GetAll()
                .Join(
                    userSharingDetailQuery,
                    us => us.Id,
                    usd => usd.UserSharingId,
                    (userSharing, userSharingDetail) => userSharing);

            var learningPahQuery = _learnerLearningPathRepository
                .GetAll();

            return
                FilterByTitleQuery(query, learningPahQuery)
                    .Join(
                        userSharingQuery,
                        lp => lp.Id,
                        us => us.ItemId,
                        (learningPath, userSharing) => learningPath);
        }
    }
}
