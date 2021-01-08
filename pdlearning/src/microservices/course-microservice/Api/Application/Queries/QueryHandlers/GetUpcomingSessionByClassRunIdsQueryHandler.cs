using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Conexus.Opal.AccessControl.Infrastructure;
using Microservice.Course.Application.Models;
using Microservice.Course.Application.Queries.Abstracts;
using Microservice.Course.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;
using Thunder.Platform.Core.Timing;

namespace Microservice.Course.Application.Queries.QueryHandlers
{
    public class GetUpcomingSessionByClassRunIdsQueryHandler : BaseQueryHandler<GetUpcomingSessionByClassRunIdsQuery, List<UpcomingSessionModel>>
    {
        private readonly IReadOnlyRepository<Session> _readSessionRepository;
        private readonly IReadOnlyRepository<AttendanceTracking> _readAttendanceTrackingRepository;
        private readonly IReadOnlyRepository<Registration> _readRegistrationRepository;

        public GetUpcomingSessionByClassRunIdsQueryHandler(
            IUserContext userContext,
            IAccessControlContext<CourseUser> accessControlContext,
            IUnitOfWorkManager unitOfWorkManager,
            IReadOnlyRepository<Session> readSessionRepository,
            IReadOnlyRepository<AttendanceTracking> readAttendanceTrackingRepository,
            IReadOnlyRepository<Registration> readRegistrationRepository)
            : base(userContext, accessControlContext, unitOfWorkManager)
        {
            _readSessionRepository = readSessionRepository;
            _readAttendanceTrackingRepository = readAttendanceTrackingRepository;
            _readRegistrationRepository = readRegistrationRepository;
        }

        protected override async Task<List<UpcomingSessionModel>> HandleAsync(GetUpcomingSessionByClassRunIdsQuery query, CancellationToken cancellationToken)
        {
            if (query.ClassRunIds == null || !query.ClassRunIds.Any())
            {
                return new List<UpcomingSessionModel>();
            }

            var sessionQuery = _readSessionRepository
                .GetAll()
                .Where(session => query.ClassRunIds.Contains(session.ClassRunId))
                .Where(session => session.StartDateTime.Value >= Clock.Now.Date);

            var attendanceTrackingQuery = _readAttendanceTrackingRepository
                .GetAll();

            var registrationQuery = _readRegistrationRepository
                .GetAll()
                .Where(p => p.UserId == CurrentUserId);

            var upcomingSessionQuery = from session in sessionQuery
                                       join registration in registrationQuery
                                           on session.ClassRunId equals registration.ClassRunId
                                       join attendanceTracking in attendanceTrackingQuery
                                           on new { SessionId = session.Id, RegistrationId = registration.Id }
                                           equals new { attendanceTracking.SessionId, attendanceTracking.RegistrationId }
                                           into attendancesTracking
                                       from attendance in attendancesTracking.DefaultIfEmpty()
                                       where attendance == null || (attendance.IsCodeScanned == false && attendance.Status == null)
                                       group session by session.ClassRunId into p
                                       select new UpcomingSessionModel
                                       {
                                           ClassRunId = p.Key,
                                           StartDateTime = p.Min(s => s.StartDateTime)
                                       };

            return await upcomingSessionQuery.ToListAsync(cancellationToken);
        }
    }
}
