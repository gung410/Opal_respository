using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microservice.Course.Application.SharedQueries.Abstractions;
using Microservice.Course.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Domain.Repositories;

namespace Microservice.Course.Application.SharedQueries
{
    public class GetMissingInfoAttendanceTrackingSharedQuery : BaseSharedQuery
    {
        private readonly IReadOnlyRepository<AttendanceTracking> _readAttendanceTrackingRepository;

        public GetMissingInfoAttendanceTrackingSharedQuery(IReadOnlyRepository<AttendanceTracking> readAttendanceTrackingRepository)
        {
            _readAttendanceTrackingRepository = readAttendanceTrackingRepository;
        }

        public async Task<List<AttendanceTracking>> ForSessions(
            IQueryable<Session> forSessions,
            CancellationToken cancellationToken)
        {
            var missingInfoAttendanceTrackingsQuery = _readAttendanceTrackingRepository.GetAll().Where(AttendanceTracking.MissingAttendanceInfoExpr());

            var missingInfoAttendanceTrackingsForSessionsQuery = missingInfoAttendanceTrackingsQuery
                .Join(forSessions, p => p.SessionId, p => p.Id, (attendanceTracking, session) => attendanceTracking);

            var missingInfoAttendanceTrackingsForSessions = await missingInfoAttendanceTrackingsForSessionsQuery.ToListAsync(cancellationToken);

            return missingInfoAttendanceTrackingsForSessions;
        }
    }
}
