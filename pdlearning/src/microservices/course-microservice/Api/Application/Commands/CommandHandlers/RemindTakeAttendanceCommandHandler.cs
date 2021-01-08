using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Conexus.Opal.AccessControl.Infrastructure;
using Microservice.Course.Application.Commands.Abstracts;
using Microservice.Course.Application.Events.Todos;
using Microservice.Course.Application.SharedQueries;
using Microservice.Course.Domain.Entities;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;
using Thunder.Platform.Core.Extensions;
using Thunder.Platform.Cqrs;

namespace Microservice.Course.Application.Commands.CommandHandlers
{
    public class RemindTakeAttendanceCommandHandler : BaseCommandHandler<RemindTakeAttendanceCommand>
    {
        private readonly IReadOnlyRepository<Session> _readSessionRepository;
        private readonly IReadOnlyRepository<CourseEntity> _readCourseRepository;
        private readonly IReadOnlyRepository<ClassRun> _readClassRunRepository;
        private readonly IThunderCqrs _thunderCqrs;
        private readonly GetMissingInfoAttendanceTrackingSharedQuery _getMissingInfoAttendanceTrackingSharedQuery;
        private readonly GetAggregatedSessionSharedQuery _getAggregatedSessionSharedQuery;
        private readonly GetCourseInvolvedUsersSharedQuery _getCourseInvolvedUsersSharedQuery;

        public RemindTakeAttendanceCommandHandler(
          IUnitOfWorkManager unitOfWorkManager,
          IUserContext userContext,
          IAccessControlContext<CourseUser> accessControlContext,
          IReadOnlyRepository<Session> readSessionRepository,
          IReadOnlyRepository<CourseEntity> readCourseRepository,
          IReadOnlyRepository<ClassRun> readClassRunRepository,
          IThunderCqrs thunderCqrs,
          GetMissingInfoAttendanceTrackingSharedQuery getMissingInfoAttendanceTrackingSharedQuery,
          GetAggregatedSessionSharedQuery getAggregatedSessionSharedQuery,
          GetCourseInvolvedUsersSharedQuery getCourseInvolvedUsersSharedQuery) : base(userContext, accessControlContext, unitOfWorkManager)
        {
            _readSessionRepository = readSessionRepository;
            _thunderCqrs = thunderCqrs;
            _getMissingInfoAttendanceTrackingSharedQuery = getMissingInfoAttendanceTrackingSharedQuery;
            _getAggregatedSessionSharedQuery = getAggregatedSessionSharedQuery;
            _getCourseInvolvedUsersSharedQuery = getCourseInvolvedUsersSharedQuery;
            _readClassRunRepository = readClassRunRepository;
            _readCourseRepository = readCourseRepository;
        }

        protected override async Task HandleAsync(RemindTakeAttendanceCommand command, CancellationToken cancellationToken)
        {
            var forSessionsQuery = _readSessionRepository
                .GetAll()
                .WhereIf(command.ForSessionEndTimeAfter.HasValue, p => p.EndDateTime >= command.ForSessionEndTimeAfter)
                .WhereIf(command.ForSessionEndTimeBefore.HasValue, p => p.EndDateTime < command.ForSessionEndTimeBefore)
                .Join(
                    _readClassRunRepository.GetAll(), p => p.ClassRunId, p => p.Id, (session, classrun) => new { session, courseId = classrun.CourseId })
                .Join(
                    _readCourseRepository.GetAll().Where(CourseEntity.IsNotArchivedExpr()), p => p.courseId, p => p.Id, (gj, course) => gj.session);

            var missingInfoAttendanceTrackings =
                await _getMissingInfoAttendanceTrackingSharedQuery.ForSessions(forSessionsQuery, cancellationToken);

            await SendTakeAttendanceNotifyLearnerEvent(missingInfoAttendanceTrackings, cancellationToken);
        }

        private async Task SendTakeAttendanceNotifyLearnerEvent(
          List<AttendanceTracking> attendanceTrackings,
          CancellationToken cancellationToken)
        {
            var aggregatedSessions =
                (await _getAggregatedSessionSharedQuery.ByIds(attendanceTrackings.Select(p => p.SessionId).ToList(), cancellationToken))
                .ToDictionary(p => p.Session.Id);
            var courseInvolvedUsers =
                await _getCourseInvolvedUsersSharedQuery.Execute(aggregatedSessions.Select(p => p.Value.Course).ToList(), cancellationToken);
            var courseInvolvedUsersDic = courseInvolvedUsers.ToDictionary(p => p.Id);
            await _thunderCqrs.SendEvents(
                attendanceTrackings
                .Select(p =>
                {
                    if (!aggregatedSessions.ContainsKey(p.SessionId)
                        || !courseInvolvedUsersDic.ContainsKey(aggregatedSessions[p.SessionId].Course.CourseFacilitatorIds.FirstOrDefault())
                        || !courseInvolvedUsersDic.ContainsKey(aggregatedSessions[p.SessionId].Course.FirstAdministratorId.GetValueOrDefault()))
                    {
                        return null;
                    }

                    var aggregatedSession = aggregatedSessions[p.SessionId];
                    return new TakeAttendanceNotifyLearnerEvent(
                        new TakeAttendanceNotifyLearnerPayload
                        {
                            CourseFacilitatorName = courseInvolvedUsersDic[aggregatedSession.Course.CourseFacilitatorIds.FirstOrDefault()].FirstName,
                            CourseFacilitatorEmail = courseInvolvedUsersDic[aggregatedSession.Course.CourseFacilitatorIds.FirstOrDefault()].Email,
                            CourseAdminName = courseInvolvedUsersDic[aggregatedSession.Course.FirstAdministratorId.GetValueOrDefault()].FirstName,
                            CourseAdminEmail = courseInvolvedUsersDic[aggregatedSession.Course.FirstAdministratorId.GetValueOrDefault()].Email,
                            SessionTitle = aggregatedSession.Session.SessionTitle,
                            ClassrunTitle = aggregatedSession.ClassRun.ClassTitle
                        },
                        new List<Guid> { p.Userid });
                })
                .Where(p => p != null)
                .ToList(),
                cancellationToken);
        }
    }
}
