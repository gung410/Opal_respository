using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Conexus.Opal.AccessControl.Extensions;
using Conexus.Opal.AccessControl.Infrastructure;
using Microservice.Course.Application.BusinessLogics.Abstractions;
using Microservice.Course.Application.Constants;
using Microservice.Course.Application.DomainExtensions;
using Microservice.Course.Domain.Entities;
using Microservice.Course.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Validation;

namespace Microservice.Course.Application.BusinessLogics
{
    public class EnsureCanChangeAttendanceTrackingLogic : BaseBusinessLogic
    {
        private readonly IReadOnlyRepository<CourseEntity> _readCourseRepository;
        private readonly IReadOnlyRepository<ClassRun> _readClassRunRepository;
        private readonly IReadOnlyRepository<Session> _readSessionRepository;
        private readonly IAccessControlContext<CourseUser> _accessControlContext;

        public EnsureCanChangeAttendanceTrackingLogic(
            IReadOnlyRepository<CourseEntity> readCourseRepository,
            IReadOnlyRepository<ClassRun> readClassRunRepository,
            IReadOnlyRepository<Session> readSessionRepository,
            IAccessControlContext<CourseUser> accessControlContext,
            IUserContext userContext) : base(userContext)
        {
            _readCourseRepository = readCourseRepository;
            _readClassRunRepository = readClassRunRepository;
            _readSessionRepository = readSessionRepository;
            _accessControlContext = accessControlContext;
        }

        public async Task EnsureCanSetPresentAbsent(
            Guid sessionId,
            CancellationToken cancellationToken = default)
        {
            var sessionQuery = _readSessionRepository.GetAll().Where(x => x.Id == sessionId);
            var classrunQuery = _readClassRunRepository.GetAll().Join(sessionQuery, p => p.Id, p => p.ClassRunId, (classrun, session) => classrun).Distinct();
            var courseQuery = _readCourseRepository
                .GetAll()
                .ApplyAccessControl(_accessControlContext)
                .Join(classrunQuery, p => p.Id, p => p.CourseId, (course, classrun) => course)
                .Distinct();

            var classRun = await classrunQuery.FirstOrDefaultAsync(cancellationToken);
            var course = await courseQuery.FirstOrDefaultAsync(cancellationToken);

            EnsureValidPermission(AttendanceTracking.HasSetPresentAbsentPermission(course, CurrentUserId, CurrentUserRoles, p => HasPermissionPrefix(p)));

            EnsureBusinessLogicValid(AttendanceTracking.ValidateCanSetPresentAbsent(course, classRun));
        }
    }
}
