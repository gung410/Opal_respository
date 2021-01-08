using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Conexus.Opal.AccessControl.Infrastructure;
using Microservice.Course.Application.BusinessLogics;
using Microservice.Course.Application.BusinessLogics.EntityCud;
using Microservice.Course.Application.Commands.Abstracts;
using Microservice.Course.Application.Constants;
using Microservice.Course.Application.SharedQueries;
using Microservice.Course.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;
using Thunder.Platform.Core.Exceptions;
using Thunder.Platform.Core.Timing;

namespace Microservice.Course.Application.Commands.CommandHandlers
{
    public class InitAttendanceTrackingForSessionCommandHandler : BaseCommandHandler<InitAttendanceTrackingForSessionCommand>
    {
        private readonly IReadOnlyRepository<Session> _readSessionRepository;
        private readonly IReadOnlyRepository<ClassRun> _readClassRunRepository;
        private readonly IReadOnlyRepository<CourseEntity> _readCourseRepository;
        private readonly GetAggregatedSessionSharedQuery _getAggregatedSessionSharedQuery;
        private readonly InitAttendanceTrackingLogic _initAttendanceTrackingLogic;

        public InitAttendanceTrackingForSessionCommandHandler(
           IUnitOfWorkManager unitOfWorkManager,
           IReadOnlyRepository<Session> readSessionRepository,
           IReadOnlyRepository<ClassRun> readClassRunRepository,
           IReadOnlyRepository<CourseEntity> readCourseRepository,
           IUserContext userContext,
           IAccessControlContext<CourseUser> accessControlContext,
           InitAttendanceTrackingLogic initAttendanceTrackingLogic,
           GetAggregatedSessionSharedQuery getAggregatedSessionSharedQuery) : base(userContext, accessControlContext, unitOfWorkManager)
        {
            _readSessionRepository = readSessionRepository;
            _getAggregatedSessionSharedQuery = getAggregatedSessionSharedQuery;
            _readClassRunRepository = readClassRunRepository;
            _readCourseRepository = readCourseRepository;
            _initAttendanceTrackingLogic = initAttendanceTrackingLogic;
        }

        protected override async Task HandleAsync(InitAttendanceTrackingForSessionCommand command, CancellationToken cancellationToken)
        {
            if (command.ForDailyTracking)
            {
                var yesterdayToTodaySessionQuery =
                    _readSessionRepository
                        .GetAll()
                        .Where(p => p.StartDateTime >= Clock.Now.AddDays(-1).Date && p.StartDateTime < Clock.Now.AddDays(1).Date)
                        .Join(_readClassRunRepository.GetAll(), p => p.ClassRunId, p => p.Id, (session, classrun) => new { session, courseId = classrun.CourseId })
                        .Join(_readCourseRepository.GetAll().Where(CourseEntity.IsNotArchivedExpr()), p => p.courseId, p => p.Id, (gj, course) => gj.session);
                await _initAttendanceTrackingLogic.Execute(yesterdayToTodaySessionQuery, cancellationToken);
            }
            else
            {
                var aggregatedSession = await _getAggregatedSessionSharedQuery.ById(command.SessionId, cancellationToken);

                EnsureBusinessLogicValid(aggregatedSession.Course.ValidateNotArchived());

                var query = _readSessionRepository.GetAll().Where(x => x.Id == command.SessionId);
                await _initAttendanceTrackingLogic.Execute(query, cancellationToken);
            }
        }
    }
}
