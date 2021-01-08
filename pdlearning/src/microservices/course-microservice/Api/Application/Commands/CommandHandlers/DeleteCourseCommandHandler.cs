using System.Threading;
using System.Threading.Tasks;
using Conexus.Opal.AccessControl.Extensions;
using Conexus.Opal.AccessControl.Infrastructure;
using Microservice.Course.Application.BusinessLogics.EntityCud;
using Microservice.Course.Application.Commands.Abstracts;
using Microservice.Course.Application.Constants;
using Microservice.Course.Domain.Entities;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Entities;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;
using Thunder.Platform.Core.Exceptions;

namespace Microservice.Course.Application.Commands.CommandHandlers
{
    public class DeleteCourseCommandHandler : BaseCommandHandler<DeleteCourseCommand>
    {
        private readonly IReadOnlyRepository<CourseEntity> _readCourseRepository;
        private readonly CourseCudLogic _courseCudLogic;

        public DeleteCourseCommandHandler(
            IUnitOfWorkManager unitOfWorkManager,
            IReadOnlyRepository<CourseEntity> readCourseRepository,
            CourseCudLogic courseCudLogic,
            IUserContext userContext,
            IAccessControlContext<CourseUser> accessControlContext) : base(userContext, accessControlContext, unitOfWorkManager)
        {
            _readCourseRepository = readCourseRepository;
            _courseCudLogic = courseCudLogic;
        }

        protected override async Task HandleAsync(DeleteCourseCommand command, CancellationToken cancellationToken)
        {
            var course = await _readCourseRepository.GetAsync(command.CourseId);

            EnsureBusinessLogicValid(course.ValidateNotArchived());

            EnsureValidPermission(
                course.HasOwnerPermission(CurrentUserId, CurrentUserRoles, _readCourseRepository.GetHasAdminRightChecker(course, AccessControlContext)));

            await _courseCudLogic.Delete(course, cancellationToken);
        }
    }
}
