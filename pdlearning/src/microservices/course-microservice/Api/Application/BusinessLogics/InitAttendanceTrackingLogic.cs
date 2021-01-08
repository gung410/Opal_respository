using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microservice.Course.Application.BusinessLogics.Abstractions;
using Microservice.Course.Application.BusinessLogics.EntityCud;
using Microservice.Course.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Timing;

namespace Microservice.Course.Application.BusinessLogics
{
    public class InitAttendanceTrackingLogic : BaseBusinessLogic
    {
        private readonly IReadOnlyRepository<AttendanceTracking> _readAttendanceTrackingRepository;
        private readonly IReadOnlyRepository<Registration> _readRegistrationRepository;
        private readonly AttendanceTrackingCudLogic _attendanceTrackingCudLogic;

        public InitAttendanceTrackingLogic(
            AttendanceTrackingCudLogic attendanceTrackingCudLogic,
            IReadOnlyRepository<Registration> readRegistrationRepository,
            IReadOnlyRepository<AttendanceTracking> readAttendanceTrackingRepository,
            IUserContext userContext) : base(userContext)
        {
            _readAttendanceTrackingRepository = readAttendanceTrackingRepository;
            _readRegistrationRepository = readRegistrationRepository;
            _attendanceTrackingCudLogic = attendanceTrackingCudLogic;
        }

        public async Task Execute(IQueryable<Session> sessionsQuery, CancellationToken cancellationToken = default)
        {
            var allRegistrationOfSessionsQuery = sessionsQuery.Join(
                _readRegistrationRepository.GetAll().Where(Registration.IsParticipantExpr()),
                p => p.ClassRunId,
                p => p.ClassRunId,
                (session, registration) => new
                {
                    SessionId = session.Id,
                    RegistrationId = registration.Id,
                    registration.UserId
                });
            var allAttendanceTrackingOfSessionsQuery = sessionsQuery
                .Join(_readAttendanceTrackingRepository.GetAll(), p => p.Id, p => p.SessionId, (session, attendanceTracking) => new
                {
                    SessionId = session.Id,
                    AttendanceTrackingId = attendanceTracking.Id,
                    attendanceTracking.RegistrationId
                });

            var notHaveAttendanceTrackingRegistrations =
                await (from registration in allRegistrationOfSessionsQuery
                       join attendanceTracking in allAttendanceTrackingOfSessionsQuery on registration.RegistrationId equals attendanceTracking.RegistrationId into atGroup
                       from at in atGroup.DefaultIfEmpty()
                       select new
                       {
                           registration.SessionId,
                           registration.RegistrationId,
                           AttendanceTrackingId = (Guid?)at.AttendanceTrackingId,
                           registration.UserId
                       })
                    .Where(p => p.AttendanceTrackingId == null)
                    .ToListAsync(cancellationToken);

            var toInsertAttendanceTrackings = notHaveAttendanceTrackingRegistrations
                .Select(p => new AttendanceTracking
                {
                    Id = Guid.NewGuid(),
                    CreatedDate = Clock.Now,
                    SessionId = p.SessionId,
                    RegistrationId = p.RegistrationId,
                    Userid = p.UserId,
                    IsCodeScanned = false
                })
                .ToList();

            await _attendanceTrackingCudLogic.InsertMany(toInsertAttendanceTrackings, cancellationToken);
        }
    }
}
