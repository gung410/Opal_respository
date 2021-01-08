using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Conexus.Opal.AccessControl.Infrastructure;
using Microservice.Course.Application.BusinessLogics.EntityCud;
using Microservice.Course.Application.Commands.Abstracts;
using Microservice.Course.Application.Constants;
using Microservice.Course.Application.SharedQueries;
using Microservice.Course.Domain.Entities;
using Microservice.Course.Domain.Enums;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Entities;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;
using Thunder.Platform.Core.Exceptions;

namespace Microservice.Course.Application.Commands.CommandHandlers
{
    public class ChangeAttendanceTrackingReasonForAbsenceCommandHandler : BaseCommandHandler<ChangeAttendanceTrackingReasonForAbsenceCommand>
    {
        private readonly IReadOnlyRepository<AttendanceTracking> _readAttendanceTrackingRepository;
        private readonly AttendanceTrackingCudLogic _attendanceTrackingCudLogic;
        private readonly GetAggregatedAttendanceTrackingSharedQuery _getAggregatedAttendanceTrackingSharedQuery;

        public ChangeAttendanceTrackingReasonForAbsenceCommandHandler(
            IUnitOfWorkManager unitOfWorkManager,
            IReadOnlyRepository<AttendanceTracking> readAttendanceTrackingRepository,
            AttendanceTrackingCudLogic attendanceTrackingCudLogic,
            IUserContext userContext,
            IAccessControlContext<CourseUser> accessControlContext,
            GetAggregatedAttendanceTrackingSharedQuery getAggregatedAttendanceTrackingSharedQuery) : base(userContext, accessControlContext, unitOfWorkManager)
        {
            _readAttendanceTrackingRepository = readAttendanceTrackingRepository;
            _attendanceTrackingCudLogic = attendanceTrackingCudLogic;
            _getAggregatedAttendanceTrackingSharedQuery = getAggregatedAttendanceTrackingSharedQuery;
        }

        protected override async Task HandleAsync(ChangeAttendanceTrackingReasonForAbsenceCommand command, CancellationToken cancellationToken)
        {
            var aggregatedAttendanceTracking = (await _getAggregatedAttendanceTrackingSharedQuery.FullByQuery(
                _readAttendanceTrackingRepository
                    .GetAll()
                    .Where(a =>
                        a.SessionId == command.SessionId &&
                        a.Userid == command.UserId &&
                        a.Status == AttendanceTrackingStatus.Absent),
                cancellationToken))
                .SingleOrDefault();

            aggregatedAttendanceTracking = EnsureEntityFound(aggregatedAttendanceTracking);
            EnsureBusinessLogicValid(aggregatedAttendanceTracking.Course.ValidateNotArchived());

            var attendanceTracking = aggregatedAttendanceTracking.AttendanceTracking;
            attendanceTracking.ReasonForAbsence = command.Reason;
            attendanceTracking.Attachment = command.Attachment;
            await _attendanceTrackingCudLogic.Update(attendanceTracking, cancellationToken);
        }
    }
}
