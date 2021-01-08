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
using Thunder.Platform.Core.Extensions;

namespace Microservice.Learner.Application.Queries.QueryHandlers
{
    public class SearchMyCourseQueryHandler :
        BaseQueryHandler<SearchMyCourseQuery, SearchPagedResultDto<CourseModel, MyCourseStatisticModel>>
    {
        private readonly IReadMyCourseShared _readMyCourseShared;
        private readonly IReadMyClassRunShared _readMyClassRunShared;
        private readonly IReadUserReviewShared _readUserReviewShared;
        private readonly IReadUserBookmarkShared _readUserBookmarkShared;
        private readonly IReadOnlyRepository<Course> _readCourseRepository;

        public SearchMyCourseQueryHandler(
            IUserContext userContext,
            IReadMyCourseShared readMyCourseShared,
            IReadMyClassRunShared readMyClassRunShared,
            IReadUserReviewShared readUserReviewShared,
            IReadUserBookmarkShared readUserBookmarkShared,
            IReadOnlyRepository<Course> readCourseRepository) : base(userContext)
        {
            _readMyCourseShared = readMyCourseShared;
            _readCourseRepository = readCourseRepository;
            _readUserReviewShared = readUserReviewShared;
            _readMyClassRunShared = readMyClassRunShared;
            _readUserBookmarkShared = readUserBookmarkShared;
        }

        protected override async Task<SearchPagedResultDto<CourseModel, MyCourseStatisticModel>> HandleAsync(
            SearchMyCourseQuery query,
            CancellationToken cancellationToken)
        {
            var searchQuery = CreateSearchQuery(query);

            // Get total count of my course items by each learning type
            var statistics =
                await CountByStatuses(query, searchQuery, cancellationToken);

            // Build my course query by my learning type
            searchQuery = FilterByStatusQuery(query, searchQuery);

            // Get total count of my course items by my learning type
            var totalCount = await searchQuery.CountAsync(cancellationToken);

            var sortCriteria = $"{nameof(MyCourse.CreatedDate)} DESC";
            searchQuery = ApplySorting(searchQuery, query.PageInfo, sortCriteria);
            searchQuery = ApplyPaging(searchQuery, query.PageInfo);

            var myCourses = await searchQuery.ToListAsync(cancellationToken);

            var courseIds = myCourses
                .Select(p => p.CourseId)
                .Distinct()
                .ToList();

            var notExpiredRegistrations =
                await _readMyClassRunShared.GetNotExpiredRegistrations(CurrentUserIdOrDefault, courseIds);

            var userBookmarks = await _readUserBookmarkShared.GetByItemIds(CurrentUserIdOrDefault, courseIds);

            var userReviewSummary = await _readUserReviewShared.GetReviewSummary(courseIds);

            var registrationIds = myCourses
                .Where(p => p.RegistrationId.HasValue)
                .Select(p => p.RegistrationId.Value)
                .ToList();

            var expiredRegistrationsDict = await _readMyClassRunShared
                .GetExpiredRegistrations(registrationIds);

            var courses = courseIds.Select(courseId =>
            {
                var myCourse = myCourses.FirstOrDefault(p => p.CourseId == courseId);

                var rating = userReviewSummary[courseId].AverageRating;
                var reviewCount = userReviewSummary[courseId].ReviewCount;

                var userBookMark = userBookmarks.FirstOrDefault(p => p.ItemId == courseId);

                var myClassRuns = notExpiredRegistrations
                    .Where(p => p.CourseId == courseId)
                    .ToList();

                var courseInfo = CourseModel
                    .New(
                        courseId,
                        rating,
                        reviewCount)
                    .WithMyCourseInfo(myCourse)
                    .WithBookmarkInfo(userBookMark);

                if (!expiredRegistrationsDict.ContainsKey(courseId))
                {
                    return courseInfo.WithMyClassRuns(myClassRuns);
                }

                var expiredMyClassRun = expiredRegistrationsDict[courseId];
                return courseInfo.WithExpiredMyClassRun(expiredMyClassRun);
            }).ToList();

            return new SearchPagedResultDto<CourseModel, MyCourseStatisticModel>(
                totalCount,
                courses,
                statistics);
        }

        /// <summary>
        /// Build search query by course name and course code.
        /// </summary>
        /// <param name="query">Request from client.</param>
        /// <returns>An <see cref="IQueryable"/> that contains the <see cref="MyCourse"/> information.</returns>
        private IQueryable<MyCourse> CreateSearchQuery(SearchMyCourseQuery query)
        {
            // 1. Creates query by CurrentUserId and CourseType condition.
            var myCourseQuery = _readMyCourseShared
                .FilterByUserIdAndCourseTypeQuery(CurrentUserIdOrDefault, query.CourseType);

            // 2. Select course by CourseName and CourseCode matching the search text.
            var courseQuery = _readCourseRepository
                .GetAll()
                .WhereIf(
                    !string.IsNullOrEmpty(query.SearchText),
                    p => (p.CourseName + p.CourseCode).Contains(query.SearchText));

            // 3. Join (3) (4) to get result
            return myCourseQuery
                .Join(
                    courseQuery,
                    mc => mc.CourseId,
                    c => c.Id,
                    (myCourse, course) => myCourse);
        }

        /// <summary>
        /// Build query by <see cref="MyLearningStatus"/>.
        /// </summary>
        /// <param name="query">Request from client.</param>
        /// <param name="searchQuery">MyCourse query.</param>
        /// <returns>An <see cref="IQueryable"/> that contains the <see cref="MyCourse"/> information.</returns>
        private IQueryable<MyCourse> FilterByStatusQuery(SearchMyCourseQuery query, IQueryable<MyCourse> searchQuery)
        {
            if (query.MyLearningStatusFilter == null)
            {
                return searchQuery;
            }

            switch (query.MyLearningStatusFilter)
            {
                case MyLearningStatus.Registered
                   when query.CourseType != LearningCourseType.Microlearning:
                    return searchQuery.Where(MyCourse.FilterRegisteredExpr());

                case MyLearningStatus.Upcoming
                   when query.CourseType != LearningCourseType.Microlearning:
                    return searchQuery.Where(MyCourse.FilterUpcomingExpr());

                case MyLearningStatus.InProgress:
                    return searchQuery.Where(MyCourse.FilterInProgressExpr());

                case MyLearningStatus.Completed:
                    return searchQuery.Where(MyCourse.FilterCompletedExpr());

                default:
                    throw new UnexpectedStatusException($"{query.MyLearningStatusFilter}");
            }
        }

        /// <summary>
        /// Get total count of my course items by each my learning type.
        /// </summary>
        /// <param name="query">Request from client.</param>
        /// <param name="searchQuery">My course query.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Returns <see cref="MyCourseStatisticModel"/> list
        /// to get total count of my course items by each <see cref="MyLearningStatus"/>.</returns>
        private async Task<List<MyCourseStatisticModel>> CountByStatuses(
            SearchMyCourseQuery query,
            IQueryable<MyCourse> searchQuery,
            CancellationToken cancellationToken)
        {
            var statistics = new List<MyCourseStatisticModel>();

            // If not include Statistic data -> we must be returned empty list data
            if (!query.IncludeStatistic)
            {
                return statistics;
            }

            var statusStatistic = await searchQuery
                .GroupBy(p => new
                {
                    p.Status
                })
                .Select(p => new
                {
                    p.Key.Status,
                    Total = p.Count()
                })
                .ToListAsync(cancellationToken);

            foreach (var learningStatus in query.MyLearningStatusStatistic)
            {
                int learningStatusCount = 0;

                switch (learningStatus)
                {
                    case MyLearningStatus.Registered
                        when query.CourseType != LearningCourseType.Microlearning:
                        learningStatusCount = statusStatistic.Sum(p => p.Total);
                        break;

                    case MyLearningStatus.Upcoming
                        when query.CourseType != LearningCourseType.Microlearning:
                        learningStatusCount = statusStatistic
                            .Where(p => p.Status == MyCourseStatus.NotStarted)
                            .Sum(p => p.Total);
                        break;

                    case MyLearningStatus.InProgress:
                        learningStatusCount = statusStatistic
                            .Where(p => p.Status == MyCourseStatus.InProgress
                                        || p.Status == MyCourseStatus.Passed)
                            .Sum(p => p.Total);
                        break;

                    case MyLearningStatus.Completed:
                        learningStatusCount = statusStatistic
                            .Where(p => p.Status == MyCourseStatus.Failed
                                        || p.Status == MyCourseStatus.Completed)
                            .Sum(p => p.Total);
                        break;

                    default:
                        throw new UnexpectedStatusException($"{learningStatus}");
                }

                statistics.Add(new MyCourseStatisticModel(learningStatus, learningStatusCount));
            }

            return statistics;
        }
    }
}
