using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Conexus.Opal.AccessControl.Infrastructure;
using Microservice.Course.Application.Models;
using Microservice.Course.Application.Queries.Abstracts;
using Microservice.Course.Domain.Entities;
using Microservice.Course.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;

namespace Microservice.Course.Application.Queries.QueryHandlers
{
    public class GetAttendanceRatioOfPresentsQueryHandler : BaseQueryHandler<GetAttendanceRatioOfPresentsQuery, List<AttendanceRatioOfPresentInfo>>
    {
        private readonly IReadOnlyRepository<AttendanceTracking> _readAttendanceTrackingRepository;
        private readonly IReadOnlyRepository<Session> _readSessionRepository;

        public GetAttendanceRatioOfPresentsQueryHandler(
            IReadOnlyRepository<AttendanceTracking> readAttendanceTrackingRepository,
            IReadOnlyRepository<Session> readSessionRepository,
            IUserContext userContext,
            IAccessControlContext<CourseUser> accessControlContext,
            IUnitOfWorkManager unitOfWorkManager) : base(userContext, accessControlContext, unitOfWorkManager)
        {
            _readAttendanceTrackingRepository = readAttendanceTrackingRepository;
            _readSessionRepository = readSessionRepository;
        }

        protected override async Task<List<AttendanceRatioOfPresentInfo>> HandleAsync(GetAttendanceRatioOfPresentsQuery query, CancellationToken cancellationToken)
        {
            var returnedList = new List<AttendanceRatioOfPresentInfo>();

            var sessionQuery = _readSessionRepository.GetAll().Where(a => a.ClassRunId.Equals(query.ClassRunId));
            var attendanceTrackingQuery = _readAttendanceTrackingRepository
                .GetAll()
                .Where(a => query.RegistrationIds.Contains(a.RegistrationId))
                .Where(AttendanceTracking.IsAttendanceCheckingCompletedExpr());
            var registrationPresentsCountDic = sessionQuery
                .Join(attendanceTrackingQuery, p => p.Id, p => p.SessionId, (session, tracking) => new
                {
                    tracking.RegistrationId
                })
                .GroupBy(p => p.RegistrationId).Select(g => new
                {
                    RegistrationId = g.Key,
                    PresentsCount = g.Count()
                })
                .ToDictionary(p => p.RegistrationId, p => p.PresentsCount);

            var totalCount = await sessionQuery.CountAsync(cancellationToken);

            return query.RegistrationIds
                .Select(p => new AttendanceRatioOfPresentInfo
                {
                    RegistrationId = p,
                    TotalSessions = totalCount,
                    PresentSessions = registrationPresentsCountDic.ContainsKey(p) ? registrationPresentsCountDic[p] : 0
                })
                .ToList();
        }
    }
}
