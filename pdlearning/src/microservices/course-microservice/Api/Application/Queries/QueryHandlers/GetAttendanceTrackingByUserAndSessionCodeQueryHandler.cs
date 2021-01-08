using System.Threading;
using System.Threading.Tasks;
using Conexus.Opal.AccessControl.Infrastructure;
using Microservice.Course.Application.Constants;
using Microservice.Course.Application.Models;
using Microservice.Course.Application.Queries.Abstracts;
using Microservice.Course.Domain.Entities;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;
using Thunder.Platform.Core.Exceptions;
using Thunder.Platform.Core.Validation;

namespace Microservice.Course.Application.Queries.QueryHandlers
{
    public class GetAttendanceTrackingByUserAndSessionCodeQueryHandler : BaseQueryHandler<GetAttendanceTrackingByUserAndSessionCodeQuery, AttendanceTrackingModel>
    {
        private readonly IReadOnlyRepository<AttendanceTracking> _readAttendanceTrackingRepository;
        private readonly IReadOnlyRepository<Session> _readSessionRepository;

        public GetAttendanceTrackingByUserAndSessionCodeQueryHandler(
            IReadOnlyRepository<AttendanceTracking> readAttendanceTrackingRepository,
            IReadOnlyRepository<Session> readSessionRepository,
            IUserContext userContext,
            IAccessControlContext<CourseUser> accessControlContext,
            IUnitOfWorkManager unitOfWorkManager) : base(userContext, accessControlContext, unitOfWorkManager)
        {
            _readAttendanceTrackingRepository = readAttendanceTrackingRepository;
            _readSessionRepository = readSessionRepository;
        }

        protected override async Task<AttendanceTrackingModel> HandleAsync(
            GetAttendanceTrackingByUserAndSessionCodeQuery query,
            CancellationToken cancellationToken)
        {
            var session = await _readSessionRepository.FirstOrDefaultAsync(x => x.SessionCode == query.SessionCode);

            EnsureBusinessLogicValid(Validation.ValidIf(session != null, "You’ve entered a wrong code."));

            var attendanceTracking = await _readAttendanceTrackingRepository.SingleAsync(x => x.Userid == CurrentUserId && x.SessionId == session.Id);

            return new AttendanceTrackingModel(attendanceTracking);
        }
    }
}
