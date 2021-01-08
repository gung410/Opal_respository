using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Conexus.Opal.AccessControl.Infrastructure;
using Microservice.Course.Application.BusinessLogics.EntityCud;
using Microservice.Course.Application.Commands.Abstracts;
using Microservice.Course.Application.SharedQueries;
using Microservice.Course.Domain.Entities;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;
using Thunder.Platform.Core.Extensions;
using Thunder.Platform.Core.Timing;

namespace Microservice.Course.Application.Commands.CommandHandlers
{
    public class CreateAttendanceTrackingForSessionCommandHandler : BaseCommandHandler<CreateAttendanceTrackingForSessionCommand>
    {
        private readonly IReadOnlyRepository<Registration> _readRegistrationRepository;
        private readonly IReadOnlyRepository<Session> _readSessionRepository;
        private readonly AttendanceTrackingCudLogic _readAttendanceTrackingCudLogic;
        private readonly GetAggregatedRegistrationSharedQuery _getAggregatedRegistrationSharedQuery;

        public CreateAttendanceTrackingForSessionCommandHandler(
           IUnitOfWorkManager unitOfWorkManager,
           IReadOnlyRepository<Registration> readRegistrationRepository,
           IReadOnlyRepository<Session> readSessionRepository,
           AttendanceTrackingCudLogic readAttendanceTrackingCudLogic,
           IUserContext userContext,
           IAccessControlContext<CourseUser> accessControlContext,
           GetAggregatedRegistrationSharedQuery getAggregatedRegistrationSharedQuery) : base(userContext, accessControlContext, unitOfWorkManager)
        {
            _readRegistrationRepository = readRegistrationRepository;
            _readSessionRepository = readSessionRepository;
            _readAttendanceTrackingCudLogic = readAttendanceTrackingCudLogic;
            _getAggregatedRegistrationSharedQuery = getAggregatedRegistrationSharedQuery;
        }

        protected override async Task HandleAsync(CreateAttendanceTrackingForSessionCommand command, CancellationToken cancellationToken)
        {
            var session = await _readSessionRepository.GetAsync(command.SessionId);
            var aggregatedRegistrations = await _getAggregatedRegistrationSharedQuery.WithClassAndCourseByRegistrationQuery(
                _readRegistrationRepository.GetAll()
                    .Where(p => p.ClassRunId == session.ClassRunId)
                    .Where(Registration.IsParticipantExpr()),
                cancellationToken);

            EnsureBusinessLogicValid(aggregatedRegistrations, p => p.Course.ValidateNotArchived());

            var registrations = aggregatedRegistrations.SelectList(p => p.Registration);
            var attendanceTrackingData = registrations
                .Select(p => new AttendanceTracking
                {
                    Id = Guid.NewGuid(),
                    CreatedDate = Clock.Now,
                    SessionId = session.Id,
                    RegistrationId = p.Id,
                    Userid = p.UserId
                })
                .ToList();

            await _readAttendanceTrackingCudLogic.InsertMany(attendanceTrackingData, cancellationToken);
        }
    }
}
