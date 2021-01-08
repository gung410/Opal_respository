using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Conexus.Opal.AccessControl.Infrastructure;
using Microservice.Course.Application.BusinessLogics.EntityCud;
using Microservice.Course.Application.Commands.Abstracts;
using Microservice.Course.Application.Constants;
using Microservice.Course.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;
using Thunder.Platform.Core.Exceptions;

namespace Microservice.Course.Application.Commands.CommandHandlers
{
    public class CloneCourseCommandHandler : BaseCommandHandler<CloneCourseCommand>
    {
        private readonly IReadOnlyRepository<CourseEntity> _readCourseRepository;
        private readonly IReadOnlyRepository<CourseUser> _readUserRepository;
        private readonly CourseCudLogic _courseCudLogic;

        public CloneCourseCommandHandler(
            IUnitOfWorkManager unitOfWorkManager,
            IReadOnlyRepository<CourseEntity> readCourseRepository,
            IReadOnlyRepository<CourseUser> readUserRepository,
            CourseCudLogic courseCudLogic,
            IUserContext userContext,
            IAccessControlContext<CourseUser> accessControlContext) : base(userContext, accessControlContext, unitOfWorkManager)
        {
            _readCourseRepository = readCourseRepository;
            _readUserRepository = readUserRepository;
            _courseCudLogic = courseCudLogic;
        }

        protected override async Task HandleAsync(CloneCourseCommand command, CancellationToken cancellationToken)
        {
            var oldCourse = await _readCourseRepository.GetAll().FirstOrDefaultAsync(p => p.Id == command.Id, cancellationToken);

            oldCourse = EnsureEntityFound(oldCourse);

            EnsureBusinessLogicValid(oldCourse.ValidateCanClone());

            var currentUser = await _readUserRepository.GetAsync(CurrentUserId.GetValueOrDefault());

            if (currentUser.DepartmentId == 0)
            {
                throw new GeneralException($"Missing departmentId of userId : {CurrentUserId} !");
            }

            EnsureValidPermission(oldCourse.HasCreatePermission(CurrentUserId, CurrentUserRoles, p => HasPermissionPrefix(p)));

            var clonedCourse = oldCourse.CloneCourse(command.NewId, CurrentUserIdOrDefault, currentUser.DepartmentId, command.FromCoursePlanning);

            await _courseCudLogic.Insert(clonedCourse, cancellationToken);
        }
    }
}
