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

namespace Microservice.Course.Application.Queries.QueryHandlers
{
    public class GetUserAttendanceTrackingsByClassRunIdHandler : BaseQueryHandler<GetUserAttendanceTrackingsByClassRunIdQuery, List<AttendanceTrackingModel>>
    {
        private readonly IReadOnlyRepository<AttendanceTracking> _readAttendanceTrackingRepository;
        private readonly IReadOnlyRepository<Session> _readSessionRepository;

        public GetUserAttendanceTrackingsByClassRunIdHandler(
            IUserContext userContext,
            IAccessControlContext<CourseUser> accessControlContext,
            IUnitOfWorkManager unitOfWorkManager,
            IReadOnlyRepository<AttendanceTracking> readAttendanceTrackingRepository,
            IReadOnlyRepository<Session> readSesionRepository) : base(userContext, accessControlContext, unitOfWorkManager)
        {
            _readAttendanceTrackingRepository = readAttendanceTrackingRepository;
            _readSessionRepository = readSesionRepository;
        }

        protected override async Task<List<AttendanceTrackingModel>> HandleAsync(GetUserAttendanceTrackingsByClassRunIdQuery query, CancellationToken cancellationToken)
        {
            var dbQuery = _readSessionRepository
                .GetAll()
                .Where(x => x.ClassRunId == query.ClassRunId)
                .Join(_readAttendanceTrackingRepository.GetAll(), s => s.Id, at => at.SessionId, (s, at) => at);

            dbQuery = dbQuery.Where(x => x.Userid == CurrentUserId).OrderByDescending(p => p.CreatedDate);

            var result = await dbQuery.Select(x => new AttendanceTrackingModel(x)).ToListAsync(cancellationToken);

            return result;
        }
    }
}
