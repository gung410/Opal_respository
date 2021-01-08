using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microservice.Learner.Application.Models;
using Microservice.Learner.Application.SharedQueries.Abstractions;
using Microservice.Learner.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Repositories;

namespace Microservice.Learner.Application.Queries.QueryHandlers
{
    public class GetMyCourseDetailsByCourseIdQueryHandler : BaseQueryHandler<GetMyCourseDetailsByCourseIdQuery, CourseModel>
    {
        private readonly IReadMyCourseShared _readMyCourseShared;
        private readonly IReadMyClassRunShared _readMyClassRunShared;
        private readonly IReadUserReviewShared _readUserReviewShared;
        private readonly IReadUserBookmarkShared _readUserBookmarkShared;
        private readonly IReadOnlyRepository<LectureInMyCourse> _readLectureInMyCourseRepository;

        public GetMyCourseDetailsByCourseIdQueryHandler(
            IUserContext userContext,
            IReadMyCourseShared readMyCourseShared,
            IReadMyClassRunShared readMyClassRunShared,
            IReadUserReviewShared readUserReviewShared,
            IReadUserBookmarkShared readUserBookmarkShared,
            IReadOnlyRepository<LectureInMyCourse> readLectureInMyCourseRepository) : base(userContext)
        {
            _readMyCourseShared = readMyCourseShared;
            _readUserReviewShared = readUserReviewShared;
            _readMyClassRunShared = readMyClassRunShared;
            _readUserBookmarkShared = readUserBookmarkShared;
            _readLectureInMyCourseRepository = readLectureInMyCourseRepository;
        }

        protected override async Task<CourseModel> HandleAsync(GetMyCourseDetailsByCourseIdQuery query, CancellationToken cancellationToken)
        {
            var existingMyCourse = await _readMyCourseShared
                .GetByUserIdAndCourseId(CurrentUserIdOrDefault, query.CourseId);

            var myLectures = new List<LectureInMyCourse>();
            if (existingMyCourse != null)
            {
                myLectures = await _readLectureInMyCourseRepository
                    .GetAll()
                    .Where(p => p.MyCourseId == existingMyCourse.Id)
                    .ToListAsync(cancellationToken);
            }

            var bookmark = await _readUserBookmarkShared
                .GetByItemId(CurrentUserIdOrDefault, query.CourseId);

            var userReviewSummary = await _readUserReviewShared.GetReviewSummary(new List<Guid> { query.CourseId });

            var rating = userReviewSummary[query.CourseId].AverageRating;
            var reviewCount = userReviewSummary[query.CourseId].ReviewCount;

            var completedTimes = await _readMyCourseShared
                .GetCompletedTimes(CurrentUserIdOrDefault, new List<Guid> { query.CourseId });

            var courseInfo = CourseModel.New(
                    query.CourseId,
                    rating,
                    reviewCount)
                .WithBookmarkInfo(bookmark);

            // For course is MicroLearning after enrollment
            if (existingMyCourse != null && existingMyCourse.IsMicroLearning())
            {
                return courseInfo
                    .WithMyCourseInfo(existingMyCourse)
                    .WithMyLecturesInfo(myLectures);
            }

            // For course is non-MicroLearning before registration
            if (existingMyCourse == null || !existingMyCourse.RegistrationId.HasValue)
            {
                return courseInfo;
            }

            var courseIds = new List<Guid> { query.CourseId };

            var notExpiredRegistrations =
                await _readMyClassRunShared.GetNotExpiredRegistrations(CurrentUserIdOrDefault, courseIds);

            var notParticipants =
                await _readMyClassRunShared.GetNotParticipants(CurrentUserIdOrDefault, courseIds);

            var rejectedMyClassRuns = notParticipants
                .Where(p => p.IsRejected())
                .ToList();

            var withdrawnMyClassRuns = notParticipants
                .Where(p => p.IsWithdrawn())
                .ToList();

            courseInfo
                .WithMyCourseInfo(existingMyCourse)
                .WithMyLecturesInfo(myLectures)
                .WithRejectedMyClassRuns(rejectedMyClassRuns)
                .WithWithdrawnMyClassRuns(withdrawnMyClassRuns)
                .WithCompletedTimes(completedTimes);

            var registrationIds = new List<Guid> { existingMyCourse.RegistrationId.Value };
            var expiredMyClassDict =
                await _readMyClassRunShared.GetExpiredRegistrations(registrationIds);

            if (!expiredMyClassDict.ContainsKey(query.CourseId))
            {
                var inProgressRegistration = notExpiredRegistrations
                    .FirstOrDefault(p => p.IsInProgress() && !p.IsFinishedLearning());

                return courseInfo
                    .WithMyClassRun(inProgressRegistration)
                    .WithMyClassRuns(notExpiredRegistrations);
            }

            var expiredMyClassRun = expiredMyClassDict[query.CourseId];

            return courseInfo
                .WithExpiredMyClassRun(expiredMyClassRun)
                .WithMyClassRunsCompleted(notExpiredRegistrations);
        }
    }
}
