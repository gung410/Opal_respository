using System.Threading;
using System.Threading.Tasks;
using Conexus.Opal.AccessControl.Infrastructure;
using Microservice.Course.Application.Models;
using Microservice.Course.Application.Queries.Abstracts;
using Microservice.Course.Domain.Entities;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;

namespace Microservice.Course.Application.Queries.QueryHandlers
{
    public class GetAttendanceTrackingByIdQueryHandler : BaseQueryHandler<GetAttendanceTrackingByIdQuery, AttendanceTrackingModel>
    {
        private readonly IReadOnlyRepository<AttendanceTracking> _readAttendanceTrackingRepository;

        public GetAttendanceTrackingByIdQueryHandler(
            IReadOnlyRepository<AttendanceTracking> readAttendanceTrackingRepository,
            IUserContext userContext,
            IAccessControlContext<CourseUser> accessControlContext,
            IUnitOfWorkManager unitOfWorkManager) : base(userContext, accessControlContext, unitOfWorkManager)
        {
            _readAttendanceTrackingRepository = readAttendanceTrackingRepository;
        }

        protected override async Task<AttendanceTrackingModel> HandleAsync(
            GetAttendanceTrackingByIdQuery query,
            CancellationToken cancellationToken)
        {
            var attendanceTracking = await _readAttendanceTrackingRepository.GetAsync(query.Id);

            return new AttendanceTrackingModel(attendanceTracking);
        }
    }
}
