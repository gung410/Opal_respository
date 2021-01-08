using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Conexus.Opal.AccessControl.Infrastructure;
using Microservice.Course.Application.BusinessLogics.EntityCud;
using Microservice.Course.Application.Commands.Abstracts;
using Microservice.Course.Application.Constants;
using Microservice.Course.Application.SharedQueries;
using Microservice.Course.Domain.Entities;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;
using Thunder.Platform.Core.Exceptions;
using Thunder.Platform.Core.Timing;

namespace Microservice.Course.Application.Commands.CommandHandlers
{
    public class UpdateTakeAttendaceCodeScannedCommandHandler : BaseCommandHandler<UpdateTakeAttendaceCodeScannedCommand>
    {
        private readonly IReadOnlyRepository<AttendanceTracking> _readAttendanceTrackingRepository;
        private readonly IReadOnlyRepository<Session> _readSessionRepository;
        private readonly GetAggregatedSessionSharedQuery _getAggregatedSessionSharedQuery;
        private readonly AttendanceTrackingCudLogic _attendanceTrackingCudLogic;

        public UpdateTakeAttendaceCodeScannedCommandHandler(
            IUnitOfWorkManager unitOfWorkManager,
            IReadOnlyRepository<AttendanceTracking> readAttendanceTrackingRepository,
            IReadOnlyRepository<Session> readSessionRepository,
            AttendanceTrackingCudLogic attendanceTrackingCudLogic,
            IUserContext userContext,
            IAccessControlContext<CourseUser> accessControlContext,
            GetAggregatedSessionSharedQuery getAggregatedSessionSharedQuery) : base(userContext, accessControlContext, unitOfWorkManager)
        {
            _readSessionRepository = readSessionRepository;
            _readAttendanceTrackingRepository = readAttendanceTrackingRepository;
            _attendanceTrackingCudLogic = attendanceTrackingCudLogic;
            _getAggregatedSessionSharedQuery = getAggregatedSessionSharedQuery;
        }

        protected override async Task HandleAsync(UpdateTakeAttendaceCodeScannedCommand command, CancellationToken cancellationToken)
        {
            var aggregatedSession = (await _getAggregatedSessionSharedQuery.ByQuery(
                _readSessionRepository.GetAll().Where(x => x.Id == command.SessionId && x.SessionCode == command.SessionCode),
                cancellationToken))
                .FirstOrDefault();

            aggregatedSession = EnsureEntityFound(aggregatedSession);

            var attendanceTracking = await _readAttendanceTrackingRepository.SingleAsync(x => x.Userid == CurrentUserId && x.SessionId == aggregatedSession.Session.Id);

            EnsureBusinessLogicValid(attendanceTracking.ValidateCanScanCodeForCheckIn(aggregatedSession.Course, aggregatedSession.Session));

            if (!attendanceTracking.IsCodeScanned)
            {
                attendanceTracking.IsCodeScanned = true;
                attendanceTracking.CodeScannedDate = Clock.Now;
                await _attendanceTrackingCudLogic.Update(attendanceTracking, cancellationToken);
            }
        }
    }
}
