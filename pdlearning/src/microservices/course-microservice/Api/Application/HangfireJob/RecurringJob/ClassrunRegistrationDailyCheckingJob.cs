using System;
using System.Linq;
using System.Threading.Tasks;
using cx.datahub.scheduling.jobs.shared;
using Microservice.Course.Application.BusinessLogics;
using Microservice.Course.Application.BusinessLogics.EntityCud;
using Microservice.Course.Application.Commands;
using Microservice.Course.Application.Constants;
using Microservice.Course.Application.Events.Todos;
using Microservice.Course.Common.Extensions;
using Microservice.Course.Domain.Entities;
using Microservice.Course.Domain.Enums;
using Microservice.Course.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;
using Thunder.Platform.Core.Extensions;
using Thunder.Platform.Core.Helpers;
using Thunder.Platform.Core.Timing;
using Thunder.Platform.Cqrs;

namespace Microservice.Course.Application.HangfireJob.RecurringJob
{
    public class ClassrunRegistrationDailyCheckingJob : BaseHangfireJob, IClassrunRegistrationDailyCheckingJob
    {
        private readonly IReadOnlyRepository<ClassRun> _readClassRunRepository;
        private readonly IReadOnlyRepository<Session> _readSessionRepository;
        private readonly IReadOnlyRepository<Registration> _readRegistrationRepository;
        private readonly RegistrationCudLogic _registrationCudLogic;
        private readonly AutoProcessLearningProgressLogic _autoProcessLearningProgressLogic;
        private readonly IReadOnlyRepository<CourseEntity> _readCourseRepository;
        private readonly WebAppLinkBuilder _webAppLinkBuilder;
        private readonly SetRegistrationsExpiredLogic _setRegistrationsExpiredLogic;
        private readonly SetAutoFailedRegistrationsLogic _setAutoFailedRegistrationsLogic;

        public ClassrunRegistrationDailyCheckingJob(
            IReadOnlyRepository<ClassRun> readClassRunRepository,
            IReadOnlyRepository<Session> readSessionRepository,
            IReadOnlyRepository<CourseEntity> readCourseRepository,
            IReadOnlyRepository<Registration> readRegistrationRepository,
            RegistrationCudLogic registrationCudLogic,
            AutoProcessLearningProgressLogic autoProcessLearningProgressLogic,
            SetRegistrationsExpiredLogic setRegistrationsExpiredLogic,
            IThunderCqrs thunderCqrs,
            IUnitOfWorkManager unitOfWorkManager,
            WebAppLinkBuilder webAppLinkBuilder,
            ILoggerFactory loggerFactory,
            SetAutoFailedRegistrationsLogic setAutoFailedRegistrationsLogic) : base(thunderCqrs, unitOfWorkManager, loggerFactory)
        {
            _readClassRunRepository = readClassRunRepository;
            _readSessionRepository = readSessionRepository;
            _readRegistrationRepository = readRegistrationRepository;
            _registrationCudLogic = registrationCudLogic;
            _readCourseRepository = readCourseRepository;
            _webAppLinkBuilder = webAppLinkBuilder;
            _setAutoFailedRegistrationsLogic = setAutoFailedRegistrationsLogic;
            _autoProcessLearningProgressLogic = autoProcessLearningProgressLogic;
            _setRegistrationsExpiredLogic = setRegistrationsExpiredLogic;
        }

        protected override async Task InternalHandleAsync()
        {
            await ByPassTaskException(ProcessExpiredRegistrations, nameof(ProcessExpiredRegistrations));
            await ByPassTaskException(ProcessAutoFailedRegistrations, nameof(ProcessAutoFailedRegistrations));
            await ByPassTaskException(ProcessLearningProgress, nameof(ProcessLearningProgress));
            await ByPassTaskException(NotifyCAOrderRefreshment, nameof(NotifyCAOrderRefreshment));
        }

        private async Task ProcessExpiredRegistrations()
        {
            var registrations = await _setRegistrationsExpiredLogic.Execute(_readClassRunRepository.GetAll(), null);

            await _registrationCudLogic.UpdateMany(registrations);
        }

        private async Task ProcessAutoFailedRegistrations()
        {
            var registrations = await _setAutoFailedRegistrationsLogic.Execute(_readClassRunRepository.GetAll());

            await _registrationCudLogic.UpdateMany(registrations);
        }

        private async Task ProcessLearningProgress()
        {
            var forSessionEndBefore = Clock.Now;

            // left join class run table with session to get class ids which ended or have last session ended
            var classrunIdsQuery = _readClassRunRepository.GetAll()
                .Where(x => !string.IsNullOrEmpty(x.ClassRunCode))
                .GroupJoin(
                    _readSessionRepository.GetAll(),
                    s => s.Id,
                    at => at.ClassRunId,
                    (classRun, session) => new { classRun, session })
                .SelectMany(
                    p => p.session.DefaultIfEmpty(),
                    (gj, session) => new
                    {
                        ClassRunId = gj.classRun.Id,
                        ClassRunEndDate = gj.classRun.EndDateTime,
                        EndDateTime = session != null ? session.EndDateTime : null
                    })
                .GroupBy(x => x.ClassRunId)
                .Select(x => new
                {
                    ClassRunId = x.Key,
                    LastSessionEndDateTime = x.Where(q => q.EndDateTime.HasValue).Max(q => q.EndDateTime),

                    // use Max here due to EF core cannot translate FirstOrDefault
                    ClassRunEndDate = x.Max(q => q.ClassRunEndDate)
                })
                .Where(x =>
                    (x.LastSessionEndDateTime.HasValue && x.LastSessionEndDateTime < forSessionEndBefore) ||
                    (x.ClassRunEndDate.HasValue && x.ClassRunEndDate < forSessionEndBefore))
                .Select(x => x.ClassRunId);

            // Get registrations which are NOT finish learning OR are required submit post course survey but not submitted in deadline
            var query = GetNeedReCalculateLearningProgressRegistrations(classrunIdsQuery);

            var updatedRegistrations = await _autoProcessLearningProgressLogic.ExecuteAsync(query);
            await _registrationCudLogic.UpdateMany(updatedRegistrations);
        }

        private async Task NotifyCAOrderRefreshment()
        {
            var courseIds = await _readClassRunRepository
                .GetAll()
                .Where(p => p.ApplicationEndDate < DateTimeHelper.StartOfTodayInSystemTimeZone().ToUtcFromSystemTimeZone() &&
                            p.ApplicationEndDate >= DateTimeHelper.StartOfTodayInSystemTimeZone().ToUtcFromSystemTimeZone().AddDays(-1))
                .Select(p => p.CourseId)
                .Distinct()
                .ToListAsync();

            var courses = await _readCourseRepository
                .GetAll()
                .Where(p => courseIds.Contains(p.Id))
                .ToListAsync();

            var events = courses.Select(course =>
                    new OrderRefreshmentNotifyCAEvent(
                        Guid.Empty,
                        new OrderRefreshmentNotifyCAPayload
                        {
                            CourseName = course.CourseName,
                            ActionUrl = _webAppLinkBuilder.GetCourseDetailLinkForCAMModule(
                                CAMTabConfigurationConstant.CoursesTab,
                                CAMTabConfigurationConstant.SendOrderRefreshmentTab,
                                null,
                                CourseDetailModeConstant.View,
                                course.Id)
                        },
                        course.GetAdministratorIds()));

            await ThunderCqrs.SendEvents(events);
        }

        private IQueryable<Registration> GetNeedReCalculateLearningProgressRegistrations(IQueryable<Guid> classrunIdsQuery)
        {
            // Query registrations which are NOT finish learning
            var notLearningFinishedRegistrationQuery = _readRegistrationRepository
                .GetAll()
                .Where(Registration.IsLearningFinishedExpr().Not())
                .Join(classrunIdsQuery, r => r.ClassRunId, crId => crId, (r, cr) => r);

            // query registrations which are required submit post course survey but not submitted in deadline
            var lateSubmitPostSurveyRegistrationQuery = _readRegistrationRepository
                .GetAll()
                .Where(x => x.LearningStatus == LearningStatus.Passed &&
                            x.SendPostCourseSurveyDate < Clock.Now.AddDays(-28));

            // need to check if registration is incomplete or passed (wait for submit post course survey)
            var query = notLearningFinishedRegistrationQuery.Union(lateSubmitPostSurveyRegistrationQuery).Distinct();
            return query;
        }
    }
}
