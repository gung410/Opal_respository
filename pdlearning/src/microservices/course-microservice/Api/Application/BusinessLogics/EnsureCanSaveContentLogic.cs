using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Conexus.Opal.AccessControl.Extensions;
using Conexus.Opal.AccessControl.Infrastructure;
using Microservice.Course.Application.BusinessLogics.Abstractions;
using Microservice.Course.Application.SharedQueries;
using Microservice.Course.Domain.Entities;
using Microservice.Course.Domain.Entities.Abstractions;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Exceptions;

namespace Microservice.Course.Application.BusinessLogics
{
    public class EnsureCanSaveContentLogic : BaseBusinessLogic
    {
        private readonly IReadOnlyRepository<CourseEntity> _readCourseRepository;
        private readonly IReadOnlyRepository<ClassRun> _readClassRunRepository;
        private readonly IAccessControlContext<CourseUser> _userAccessControlContext;
        private readonly GetHasLearnerStartedSharedQuery _getHasLearnerStartedSharedQuery;

        public EnsureCanSaveContentLogic(
            IReadOnlyRepository<CourseEntity> readCourseRepository,
            IReadOnlyRepository<ClassRun> readClassRunRepository,
            GetHasLearnerStartedSharedQuery getHasLearnerStartedSharedQuery,
            IAccessControlContext<CourseUser> accessControlContext,
            IUserContext userContext) : base(userContext)
        {
            _readCourseRepository = readCourseRepository;
            _readClassRunRepository = readClassRunRepository;
            _getHasLearnerStartedSharedQuery = getHasLearnerStartedSharedQuery;
            _userAccessControlContext = accessControlContext;
        }

        public async Task<ValueTuple<CourseEntity, ClassRun>> Execute(
            Guid courseId,
            Guid? classrunId,
            BaseContent toModifyContent = null)
        {
            var course = await _readCourseRepository.GetAsync(courseId);
            var classrun = classrunId != null ? await _readClassRunRepository.GetAsync(classrunId.Value) : null;

            var hasAdminRightChecker = _readCourseRepository.GetHasAdminRightChecker(course, _userAccessControlContext);
            EnsureValidPermission(toModifyContent != null
                ? toModifyContent.HasModifyPermission(CurrentUserId, CurrentUserRoles, course, hasAdminRightChecker, p => HasPermissionPrefix(p), classrun)
                : course.HasModifyCourseContentPermission(CurrentUserId, CurrentUserRoles, hasAdminRightChecker, p => HasPermissionPrefix(p)));

            bool hasLearnerStarted = classrun != null && await _getHasLearnerStartedSharedQuery.ByClassRunId(classrun.Id);
            EnsureBusinessLogicValid(classrun != null
                ? classrun.ValidateCanEditContent(course, hasLearnerStarted)
                : course.ValidateCanEditContent());

            return (course, classrun);
        }
    }
}
