using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Conexus.Opal.AccessControl.Infrastructure;
using Microservice.Course.Application.AggregatedEntityModels;
using Microservice.Course.Application.BusinessLogics;
using Microservice.Course.Application.BusinessLogics.EntityCud;
using Microservice.Course.Application.Commands.Abstracts;
using Microservice.Course.Application.SharedQueries;
using Microservice.Course.Domain.Entities;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;
using Thunder.Platform.Core.Extensions;

namespace Microservice.Course.Application.Commands.CommandHandlers
{
    public class ChangeAttendancesStatusCommandHandler : BaseCommandHandler<ChangeAttendancesStatusCommand>
    {
        private readonly IReadOnlyRepository<AttendanceTracking> _readAttendanceTrackingRepository;
        private readonly EnsureCanChangeAttendanceTrackingLogic _ensureCanChangeAttendanceTrackingLogic;
        private readonly GetAggregatedAttendanceTrackingSharedQuery _getAggregatedAttendanceTrackingSharedQuery;
        private readonly AutoProcessLearningProgressLogic _autoProcessLearningProgressLogic;
        private readonly AttendanceTrackingCudLogic _attendanceTrackingCudLogic;
        private readonly RegistrationCudLogic _registrationCudLogic;

        public ChangeAttendancesStatusCommandHandler(
           IUnitOfWorkManager unitOfWorkManager,
           IReadOnlyRepository<AttendanceTracking> readAttendanceTrackingRepository,
           AttendanceTrackingCudLogic attendanceTrackingCudLogic,
           RegistrationCudLogic registrationCudLogic,
           IUserContext userContext,
           IAccessControlContext<CourseUser> accessControlContext,
           EnsureCanChangeAttendanceTrackingLogic ensureCanChangeAttendanceTrackingLogic,
           GetAggregatedAttendanceTrackingSharedQuery getAggregatedAttendanceTrackingSharedQuery,
           AutoProcessLearningProgressLogic autoProcessLearningProgressLogic) : base(userContext, accessControlContext, unitOfWorkManager)
        {
            _readAttendanceTrackingRepository = readAttendanceTrackingRepository;
            _ensureCanChangeAttendanceTrackingLogic = ensureCanChangeAttendanceTrackingLogic;
            _getAggregatedAttendanceTrackingSharedQuery = getAggregatedAttendanceTrackingSharedQuery;
            _autoProcessLearningProgressLogic = autoProcessLearningProgressLogic;
            _attendanceTrackingCudLogic = attendanceTrackingCudLogic;
            _registrationCudLogic = registrationCudLogic;
        }

        protected override async Task HandleAsync(ChangeAttendancesStatusCommand command, CancellationToken cancellationToken)
        {
            await _ensureCanChangeAttendanceTrackingLogic.EnsureCanSetPresentAbsent(command.SessionId, cancellationToken);

            var aggregatedAttendanceTrackings = await _getAggregatedAttendanceTrackingSharedQuery.WithRegistrationByQuery(
                _readAttendanceTrackingRepository.GetAll().Where(p => command.AttendanceTrackingIds.Contains(p.Id)),
                cancellationToken);

            var updatedAggregatedAttendanceTrackings = await UpdateStatusForAttendanceTrackings(command, aggregatedAttendanceTrackings, cancellationToken);
            await ProcessParticipantLearningProgress(updatedAggregatedAttendanceTrackings, cancellationToken);
        }

        private async Task ProcessParticipantLearningProgress(
            List<AttendanceTrackingAggregatedEntityModel> updatedAggregatedAttendanceTrackings,
            CancellationToken cancellationToken)
        {
            var registrations = updatedAggregatedAttendanceTrackings.SelectList(p => p.Registration);
            var updatedAttendanceTrackings = updatedAggregatedAttendanceTrackings.SelectList(p => p.AttendanceTracking);

            var processedLearningProgressRegistrations = await _autoProcessLearningProgressLogic.ExecuteAsync(
                registrations,
                updatedAttendanceTrackings,
                cancellationToken);

            await _registrationCudLogic.UpdateMany(processedLearningProgressRegistrations, cancellationToken);
        }

        private async Task<List<AttendanceTrackingAggregatedEntityModel>> UpdateStatusForAttendanceTrackings(
            ChangeAttendancesStatusCommand command,
            List<AttendanceTrackingAggregatedEntityModel> aggregatedAttendanceTrackings,
            CancellationToken cancellationToken)
        {
            var attendanceTrackings = aggregatedAttendanceTrackings.SelectList(p => p.AttendanceTracking);

            var updatedAttendanceTrackings = attendanceTrackings.SelectList(p =>
            {
                p.Status = command.Status;
                return p;
            });

            await _attendanceTrackingCudLogic.UpdateMany(updatedAttendanceTrackings, cancellationToken);

            return aggregatedAttendanceTrackings;
        }
    }
}
