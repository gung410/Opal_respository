using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microservice.Learner.Application.Models;
using Microservice.Learner.Application.SharedQueries.Abstractions;
using Microservice.Learner.Domain.Entities;
using Microservice.Learner.Domain.ValueObject;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Application.Dtos;
using Thunder.Platform.Core.Context;

namespace Microservice.Learner.Application.Queries.QueryHandlers
{
    public class GetMyCoursesQueryHandler : BaseQueryHandler<GetMyCoursesQuery, PagedResultDto<CourseModel>>
    {
        private readonly IReadMyCourseShared _readMyCourseShared;
        private readonly IReadUserBookmarkShared _readUserBookmarkShared;

        public GetMyCoursesQueryHandler(
            IUserContext userContext,
            IReadMyCourseShared readMyCourseShared,
            IReadUserBookmarkShared readUserBookmarkShared) : base(userContext)
        {
            _readMyCourseShared = readMyCourseShared;
            _readUserBookmarkShared = readUserBookmarkShared;
        }

        protected override async Task<PagedResultDto<CourseModel>> HandleAsync(GetMyCoursesQuery query, CancellationToken cancellationToken)
        {
            // Build my course by UserId and CourseType condition.
            var myCourseQuery = _readMyCourseShared
                .FilterByUserIdAndCourseTypeQuery(CurrentUserIdOrDefault, query.CourseType);

            switch (query.StatusFilter)
            {
                // For course is non MicroLearning
                case MyLearningStatus.Registered
                    when query.CourseType != LearningCourseType.Microlearning:
                    myCourseQuery = myCourseQuery.Where(MyCourse.FilterRegisteredExpr());
                    break;

                // For course is non MicroLearning
                case MyLearningStatus.Upcoming
                    when query.CourseType != LearningCourseType.Microlearning:
                    myCourseQuery = myCourseQuery.Where(MyCourse.FilterUpcomingExpr());
                    break;

                // For both courses is MicroLearning and non MicroLearning
                case MyLearningStatus.InProgress:
                    myCourseQuery = myCourseQuery.Where(MyCourse.FilterInProgressExpr());
                    break;

                // For both MicroLearning and non MicroLearning courses
                case MyLearningStatus.Completed:
                    myCourseQuery = myCourseQuery.Where(MyCourse.FilterCompletedExpr());
                    break;

                default:
                    throw new NotSupportedException();
            }

            var totalCount = await myCourseQuery.CountAsync(cancellationToken);

            myCourseQuery = ApplySorting(myCourseQuery, query.PageInfo, $"{query.OrderBy}");
            myCourseQuery = ApplyPaging(myCourseQuery, query.PageInfo);

            var courses = await myCourseQuery.ToListAsync(cancellationToken);
            var courseIds = courses.Select(p => p.CourseId).ToList();

            var bookmarks = await _readUserBookmarkShared
                .GetByItemIds(CurrentUserIdOrDefault, courseIds);

            var courseItems = await _readMyCourseShared
                .GetRelatedInfoOfCourses(
                    CurrentUserIdOrDefault,
                    courseIds,
                    bookmarks,
                    courses);

            return new PagedResultDto<CourseModel>(totalCount, courseItems);
        }
    }
}
