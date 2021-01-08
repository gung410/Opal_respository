using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microservice.Course.Application.BusinessLogics.Abstractions;
using Microservice.Course.Application.Events.Todos;
using Microservice.Course.Domain.Entities;
using Microservice.Course.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Timing;
using Thunder.Platform.Core.Validation;
using Thunder.Platform.Cqrs;

namespace Microservice.Course.Application.BusinessLogics
{
    public class ArchiveCourseLogic : BaseBusinessLogic
    {
        private readonly IReadOnlyRepository<CourseEntity> _readCourseRepository;
        private readonly IWriteOnlyRepository<CourseEntity> _writeCourseRepository;
        private readonly IReadOnlyRepository<ClassRun> _readClassRunRepository;
        private readonly IReadOnlyRepository<Registration> _readRegistrationRepository;
        private readonly IReadOnlyRepository<LearningPathCourse> _readLearningPathCourseRepository;
        private readonly IReadOnlyRepository<LearningPath> _readLearningPathRepository;
        private readonly IThunderCqrs _thunderCqrs;

        public ArchiveCourseLogic(
            IReadOnlyRepository<ClassRun> readClassRunRepository,
            IReadOnlyRepository<Registration> readRegistrationRepository,
            IReadOnlyRepository<LearningPathCourse> readLearningPathCourseRepository,
            IReadOnlyRepository<LearningPath> readLearningPathRepository,
            IReadOnlyRepository<CourseEntity> readCourseRepository,
            IWriteOnlyRepository<CourseEntity> writeCourseRepository,
            IThunderCqrs thunderCqrs,
            IUserContext userContext) : base(userContext)
        {
            _readRegistrationRepository = readRegistrationRepository;
            _readLearningPathCourseRepository = readLearningPathCourseRepository;
            _readLearningPathRepository = readLearningPathRepository;
            _thunderCqrs = thunderCqrs;
            _readCourseRepository = readCourseRepository;
            _writeCourseRepository = writeCourseRepository;
            _readClassRunRepository = readClassRunRepository;
        }

        public async Task ArchiveCourses(List<CourseEntity> courses, Guid userId, CancellationToken cancellationToken = default)
        {
            courses.ForEach(p =>
            {
                p.ArchivedBy = userId;
                p.ArchiveDate = Clock.Now;
                p.ChangedBy = userId;
                p.Status = CourseStatus.Archived;
            });
            await _writeCourseRepository.UpdateManyAsync(courses);
            await SendArchivedCourseNotificationEvent(courses, userId, cancellationToken);
        }

        public IQueryable<CourseEntity> FilterCanArchiveCourses(IQueryable<CourseEntity> query)
        {
            return query
                .Where(CourseEntity.CanBeArchivedExpr(false))
                .GroupJoin(
                    _readRegistrationRepository.GetAll().Where(Registration.IsLearningInProgressParticipantExpr()),
                    p => p.Id,
                    p => p.CourseId,
                    (course, registrations) => new { course, registrations })
                .SelectMany(
                    p => p.registrations.DefaultIfEmpty(),
                    (gj, registration) => new { gj.course, registration })
                .Where(p => p.registration == null)
                .Select(p => p.course)
                .Distinct();
        }

        public Task<List<CourseEntity>> GetCanAutoArchiveCoursesByQuery(
            IQueryable<CourseEntity> query,
            CancellationToken cancellationToken = default)
        {
            // Get course with most recent changed date. The date is the most recent date between course created/changed date and classrun created/changed dates
            // Filter course which have passed planning archive date OR there is no changed for at least 1 year since expired date
            return
                FilterCanArchiveCourses(query)
                    .GroupJoin(
                        _readClassRunRepository.GetAll(),
                        p => p.Id,
                        p => p.CourseId,
                        (course, classRuns) => new { course, classRuns })
                    .SelectMany(
                        p => p.classRuns.DefaultIfEmpty(),
                        (gj, classRun) => new
                        {
                            gj.course,
                            classRunMostRecentChangedDate = classRun != null
                                ? (classRun.ChangedDate ?? classRun.CreatedDate)
                                : (DateTime?)null,
                            courseMostRecentChangedDate = gj.course != null
                                ? (gj.course.ChangedDate ?? gj.course.CreatedDate)
                                : (DateTime?)null
                        })
                    .GroupBy(
                        p => p.course.Id,
                        (courseId, gj) => new
                        {
                            courseId,
                            classRunMostRecentChangedDate = gj.Max(x => x.classRunMostRecentChangedDate),
                            courseMostRecentChangedDate = gj.Max(x => x.courseMostRecentChangedDate)
                        })
                    .Select(p => new
                    {
                        p.courseId,
                        mostRecentChangedDate = p.classRunMostRecentChangedDate > p.courseMostRecentChangedDate
                            ? p.classRunMostRecentChangedDate
                            : p.courseMostRecentChangedDate
                    })
                    .Join(_readCourseRepository.GetAll(), p => p.courseId, p => p.Id, (grp1, course) => new { course, grp1.mostRecentChangedDate })
                    .Where(p =>
                        (p.course.PlanningArchiveDate != null && p.course.PlanningArchiveDate <= Clock.Now) ||
                        (p.course.ExpiredDate != null &&
                         p.course.ExpiredDate <= Clock.Now.AddYears(-1) &&
                         p.course.ExpiredDate > p.mostRecentChangedDate))
                    .Select(p => p.course)
                    .ToListAsync(cancellationToken);
        }

        public async Task EnsureCanArchiveCourses(
            List<CourseEntity> courses,
            CancellationToken cancellationToken = default)
        {
            // TODO: The following code is nightmare with readability and interpretation.
            var courseIds = courses.Select(p => p.Id);

            // Get id of courses which have learning in progress learner
            var hasInProgressLearnerCourseIds = (await _readRegistrationRepository
                .GetAll()
                .Where(Registration.IsLearningInProgressParticipantExpr())
                .Where(p => courseIds.Contains(p.CourseId))
                .Select(p => p.CourseId)
                .ToListAsync(cancellationToken))
                .ToHashSet();

            EnsureBusinessLogicValid(
                courses,
                course => CourseEntity
                    .CanArchiveCourseValidator(isAnyInProgressLearner: hasInProgressLearnerCourseIds.Contains(course.Id))
                    .Validate(course));
        }

        private async Task SendArchivedCourseNotificationEvent(List<CourseEntity> courses, Guid userId, CancellationToken cancellationToken)
        {
            // TODO: The following code is nightmare with readability and interpretation.
            var courseToLearningPathCreatorIdsDict = (await
                    _readLearningPathCourseRepository
                        .GetAll()
                        .Where(p => courses.Select(x => x.Id).Contains(p.CourseId))
                        .Join(
                            _readLearningPathRepository.GetAll(),
                            p => p.LearningPathId,
                            p => p.Id,
                            (learningPathCourse, learningPath) => new
                            {
                                LearningPathCourseCourseId = learningPathCourse.CourseId,
                                LearningPathCreatedBy = learningPath.CreatedBy
                            })
                        .ToListAsync(cancellationToken))
                .GroupBy(p => p.LearningPathCourseCourseId)
                .ToDictionary(p => p.Key, p => p.Select(x => x.LearningPathCreatedBy).ToList());
            var events = courses
                .Select(course =>
                    new ArchivedCourseNotificationEvent(
                        userId,
                        new ArchivedCourseNotificationPayload
                        {
                            CourseTitle = course.CourseName,
                            CourseType = course.CourseType.ToString()
                        },
                        course.GetManagementInvoledUserIds()
                            .Concat(courseToLearningPathCreatorIdsDict.GetValueOrDefault(course.Id) ?? new List<Guid>())
                            .Where(p => p != userId)
                            .Distinct()
                            .ToList()))
                .Where(p => p.AssignedToIds.Any())
                .ToList();
            await _thunderCqrs.SendEvents(events, cancellationToken);
        }
    }
}
