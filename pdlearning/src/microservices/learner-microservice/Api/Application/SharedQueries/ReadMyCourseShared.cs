using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microservice.Learner.Application.Models;
using Microservice.Learner.Application.SharedQueries.Abstractions;
using Microservice.Learner.Domain.Entities;
using Microservice.Learner.Domain.ValueObject;
using Microservice.Learner.Extensions;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Extensions;

namespace Microservice.Learner.Application.SharedQueries
{
    public class ReadMyCourseShared : BaseReadShared<MyCourse>, IReadMyCourseShared
    {
        private readonly IReadMyClassRunShared _readMyClassRunShared;
        private readonly IReadUserReviewShared _readUserReviewShared;

        public ReadMyCourseShared(
            IReadMyClassRunShared readMyClassRunShared,
            IReadUserReviewShared readUserReviewShared,
            IReadOnlyRepository<MyCourse> readMyCourseRepository) : base(readMyCourseRepository)
        {
            _readUserReviewShared = readUserReviewShared;
            _readMyClassRunShared = readMyClassRunShared;
        }

        public Task<int> GetCompletedTimes(Guid userId, List<Guid> courseIds)
        {
            return ReadRepository
                .GetAll()
                .Where(p => p.UserId == userId)
                .Where(p => courseIds.Contains(p.CourseId))
                .Where(p => p.Status == MyCourseStatus.Completed)
                .CountAsync();
        }

        public Task<MyCourse> GetByUserIdAndCourseId(Guid userId, Guid courseId)
        {
            return ReadRepository
                .GetAll()
                .Where(p => p.UserId == userId)
                .Where(p => p.CourseId == courseId)
                .OrderByDescending(p => p.CreatedDate)
                .FirstOrDefaultAsync();
        }

        public Task<List<MyCourse>> GetByUserIdAndCourseIds(Guid userId, List<Guid> courseIds)
        {
            var predicate =
                MyCourse.FilterByUserIdExpr(userId).AndAlso(MyCourse.FilterByCourseIdsExpr(courseIds));

            return CreateQueryWithInnerPredicate(predicate).ToListAsync();
        }

        public IQueryable<MyCourse> FilterByRegistrationIdsQuery(List<Guid> registrationIds)
        {
            return ReadRepository
                .GetAll()
                .Where(p => registrationIds.Contains(p.RegistrationId.Value));
        }

        public IQueryable<MyCourse> FilterByCourseIdAndCourseTypeQuery(Guid courseId, LearningCourseType courseType)
        {
            var predicate =
                MyCourse.FilterByCourseIdsExpr(new List<Guid> { courseId }).AndAlso(MyCourse.FilterByCourseTypeExpr(courseType));

            return CreateQueryWithInnerPredicate(predicate);
        }

        public IQueryable<MyCourse> FilterByUserIdAndCourseTypeQuery(Guid userId, LearningCourseType courseType)
        {
            var predicate =
                MyCourse.FilterByUserIdExpr(userId).AndAlso(MyCourse.FilterByCourseTypeExpr(courseType));

            return CreateQueryWithInnerPredicate(predicate);
        }

        public IQueryable<MyCourse> FilterByUserIdAndStatusQuery(Guid userId, List<MyCourseStatus> statuses)
        {
            var predicate =
                MyCourse.FilterByUserIdExpr(userId).AndAlso(MyCourse.FilterByStatusExpr(statuses));

            return CreateQueryWithInnerPredicate(predicate);
        }

        public async Task<List<CourseModel>> GetRelatedInfoOfCourses(
            Guid userId,
            List<Guid> courseIds,
            List<UserBookmark> userBookmarks,
            List<MyCourse> myCourses)
        {
            var courseIdList = myCourses.Select(p => p.CourseId).ToList();

            var notParticipants =
                await _readMyClassRunShared.GetNotParticipants(userId, courseIdList);

            var notExpiredRegistrations =
                await _readMyClassRunShared.GetNotExpiredRegistrations(userId, courseIdList);

            var registrationIds = myCourses
                .Where(p => p.RegistrationId.HasValue)
                .Select(p => p.RegistrationId.Value)
                .ToList();

            var expiredMyClassDict = await _readMyClassRunShared.GetExpiredRegistrations(registrationIds);

            var userReviewSummary = await _readUserReviewShared.GetReviewSummary(courseIds);

            var courses = new List<CourseModel>();

            foreach (var courseId in courseIds)
            {
                var myCourse = myCourses.FirstOrDefault(p => p.CourseId == courseId);

                var userBookmark = userBookmarks.FirstOrDefault(bookmark => bookmark.ItemId == courseId);

                var notExpiredRegistrationsByCourseId = notExpiredRegistrations
                    .Where(p => p.CourseId == courseId)
                    .ToList();

                // Get latest MyClassRun if current user have multiple MyClassRun InProgress.
                var inProgressRegistration = notExpiredRegistrationsByCourseId
                    .Where(p => p.IsInProgress() && !p.IsFinishedLearning())
                    .OrderByDescending(p => p.CreatedDate)
                    .FirstOrDefault();

                var rejectedMyClassRuns = notParticipants
                    .Where(p => p.CourseId == courseId && p.IsRejected())
                    .ToList();

                var withdrawnMyClassRuns = notParticipants
                    .Where(p => p.CourseId == courseId && p.IsWithdrawn())
                    .ToList();

                var courseInfo = CourseModel.New(
                        courseId,
                        userReviewSummary[courseId].AverageRating,
                        userReviewSummary[courseId].ReviewCount)
                    .WithMyCourseInfo(myCourse)
                    .WithBookmarkInfo(userBookmark)
                    .WithRejectedMyClassRuns(rejectedMyClassRuns)
                    .WithWithdrawnMyClassRuns(withdrawnMyClassRuns);

                if (!expiredMyClassDict.ContainsKey(courseId))
                {
                    courseInfo
                        .WithMyClassRun(inProgressRegistration)
                        .WithMyClassRuns(notExpiredRegistrationsByCourseId);

                    courses.Add(courseInfo);
                    continue;
                }

                var expiredMyClassRun = expiredMyClassDict[courseId];
                courseInfo
                    .WithExpiredMyClassRun(expiredMyClassRun);

                courses.Add(courseInfo);
            }

            return courses;
        }

        private IQueryable<MyCourse> CreateQueryWithInnerPredicate(Expression<Func<MyCourse, bool>> innerPredicate)
        {
            var innerQuery = ReadRepository
                .GetAll()
                .WhereIf(innerPredicate != null, innerPredicate)
                .GroupBy(p => new
                {
                    p.UserId,
                    p.CourseId
                })
                .Select(p => new
                {
                    p.Key,
                    CreatedDate = p.Max(x => x.CreatedDate)
                });

            return ReadRepository
                .GetAll()
                .Join(
                    innerQuery,
                    outer => new
                    {
                        outer.CourseId,
                        outer.UserId,
                        outer.CreatedDate
                    },
                    inner => new
                    {
                        inner.Key.CourseId,
                        inner.Key.UserId,
                        inner.CreatedDate
                    },
                    (myCourseOuter, myCourseInner) => myCourseOuter);
        }
    }
}
