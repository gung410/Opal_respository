using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Conexus.Opal.AccessControl.Infrastructure;
using Conexus.Opal.Connector.RabbitMQ.Contract;
using Microservice.Course.Application.BusinessLogics.EntityCud;
using Microservice.Course.Application.Commands.Abstracts;
using Microservice.Course.Application.Events.Todos;
using Microservice.Course.Application.SharedQueries;
using Microservice.Course.Domain.Entities;
using Microservice.Course.Domain.Enums;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;
using Thunder.Platform.Core.Extensions;
using Thunder.Platform.Core.Timing;
using Thunder.Platform.Cqrs;

namespace Microservice.Course.Application.Commands.CommandHandlers
{
    public class SetAbsentForMissingInfoAttendanceTrackingCommandHandler : BaseCommandHandler<SetAbsentForMissingInfoAttendanceTrackingCommand>
    {
        private readonly IReadOnlyRepository<Session> _readSessionRepository;
        private readonly IReadOnlyRepository<ClassRun> _readClassRunRepository;
        private readonly IReadOnlyRepository<CourseEntity> _readCourseRepository;
        private readonly IThunderCqrs _thunderCqrs;
        private readonly GetMissingInfoAttendanceTrackingSharedQuery _getMissingInfoAttendanceTrackingSharedQuery;
        private readonly GetAggregatedSessionSharedQuery _getAggregatedSessionSharedQuery;
        private readonly GetCourseInvolvedUsersSharedQuery _getCourseInvolvedUsersSharedQuery;
        private readonly AttendanceTrackingCudLogic _attendanceTrackingCudLogic;

        public SetAbsentForMissingInfoAttendanceTrackingCommandHandler(
           IUnitOfWorkManager unitOfWorkManager,
           IReadOnlyRepository<Session> readSessionRepository,
           IReadOnlyRepository<ClassRun> readClassRunRepository,
           IReadOnlyRepository<CourseEntity> readCourseRepository,
           IThunderCqrs thunderCqrs,
           IUserContext userContext,
           IAccessControlContext<CourseUser> accessControlContext,
           GetMissingInfoAttendanceTrackingSharedQuery getMissingInfoAttendanceTrackingSharedQuery,
           GetAggregatedSessionSharedQuery getAggregatedSessionSharedQuery,
           GetCourseInvolvedUsersSharedQuery getCourseInvolvedUsersSharedQuery,
           AttendanceTrackingCudLogic attendanceTrackingCudLogic) : base(userContext, accessControlContext, unitOfWorkManager)
        {
            _readSessionRepository = readSessionRepository;
            _thunderCqrs = thunderCqrs;
            _getMissingInfoAttendanceTrackingSharedQuery = getMissingInfoAttendanceTrackingSharedQuery;
            _getAggregatedSessionSharedQuery = getAggregatedSessionSharedQuery;
            _getCourseInvolvedUsersSharedQuery = getCourseInvolvedUsersSharedQuery;
            _attendanceTrackingCudLogic = attendanceTrackingCudLogic;
            _readClassRunRepository = readClassRunRepository;
            _readCourseRepository = readCourseRepository;
        }

        protected override async Task HandleAsync(SetAbsentForMissingInfoAttendanceTrackingCommand command, CancellationToken cancellationToken)
        {
            var forSessionsQuery = _readSessionRepository
                .GetAll()
                .WhereIf(command.ForSessionStartAfter.HasValue, p => p.StartDateTime >= command.ForSessionStartAfter)
                .WhereIf(command.ForSessionStartBefore.HasValue, p => p.StartDateTime < command.ForSessionStartBefore)
                .Join(
                    _readClassRunRepository.GetAll(), p => p.ClassRunId, p => p.Id, (session, classrun) => new { session, classrun.CourseId })
                .Join(
                    _readCourseRepository.GetAll().Where(CourseEntity.IsNotArchivedExpr()), p => p.CourseId, p => p.Id, (gj, course) => gj.session);

            var missingInfoAttendanceTrackings
                = await _getMissingInfoAttendanceTrackingSharedQuery.ForSessions(forSessionsQuery, cancellationToken);

            missingInfoAttendanceTrackings.ForEach(p => p.Status = AttendanceTrackingStatus.Absent);

            await _attendanceTrackingCudLogic.UpdateMany(missingInfoAttendanceTrackings, cancellationToken);

            await SendAbsenceNotifyLearnerEvent(missingInfoAttendanceTrackings, cancellationToken);
        }

        private async Task SendAbsenceNotifyLearnerEvent(
          List<AttendanceTracking> attendanceTrackings,
          CancellationToken cancellationToken)
        {
            var aggregatedSessions =
                (await _getAggregatedSessionSharedQuery.ByIds(attendanceTrackings.Select(p => p.SessionId).ToList(), cancellationToken))
                .ToDictionary(p => p.Session.Id);
            var courseInvolvedUsers = await _getCourseInvolvedUsersSharedQuery.Execute(aggregatedSessions.Select(p => p.Value.Course).ToList(), cancellationToken);
            var courseInvolvedUsersDic = courseInvolvedUsers.ToDictionary(p => p.Id);

            DateTime remindedUtcDateTime = Clock.Now.AddHours(9);

            var events = new List<BaseThunderEvent>();
            attendanceTrackings.ForEach(p =>
            {
                var aggregatedSession = aggregatedSessions[p.SessionId];
                events.Add(new AbsenceNotifyLearnerEvent(
                    new AbsenceNotifyLearnerPayload
                    {
                        CourseFacilitatorName = courseInvolvedUsersDic[aggregatedSession.Course.CourseFacilitatorIds.FirstOrDefault()].FirstName,
                        CourseFacilitatorEmail = courseInvolvedUsersDic[aggregatedSession.Course.CourseFacilitatorIds.FirstOrDefault()].Email,
                        CourseAdminName = courseInvolvedUsersDic[aggregatedSession.Course.FirstAdministratorId.GetValueOrDefault()].FirstName,
                        CourseAdminEmail = courseInvolvedUsersDic[aggregatedSession.Course.FirstAdministratorId.GetValueOrDefault()].Email,
                        SessionTitle = aggregatedSession.Session.SessionTitle,
                        ClassrunTitle = aggregatedSession.ClassRun.ClassTitle
                    },
                    new List<Guid> { p.Userid },
                    new ReminderByDto
                    {
                        Type = ReminderByType.AbsoluteDateTimeUTC,
                        Value = remindedUtcDateTime.ToString(CultureInfo.InvariantCulture)
                    }));
            });
            await _thunderCqrs.SendEvents(events, cancellationToken);
        }
    }
}
