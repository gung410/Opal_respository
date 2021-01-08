using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Conexus.Opal.AccessControl.Extensions;
using Conexus.Opal.AccessControl.Infrastructure;
using Microservice.Course.Application.BusinessLogics.EntityCud;
using Microservice.Course.Application.Commands.Abstracts;
using Microservice.Course.Domain.Entities;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;

namespace Microservice.Course.Application.Commands.CommandHandlers
{
    public class TransferCourseOwnershipCommandHandler : BaseCommandHandler<TransferCourseOwnershipCommand>
    {
        private readonly IReadOnlyRepository<CourseEntity> _readCourseRepository;
        private readonly IReadOnlyRepository<CourseUser> _readCourseUserRepository;
        private readonly CourseCudLogic _courseCudLogic;

        public TransferCourseOwnershipCommandHandler(
            IUnitOfWorkManager unitOfWorkManager,
            IReadOnlyRepository<CourseEntity> readCourseRepository,
            IReadOnlyRepository<CourseUser> readCourseUserRepository,
            CourseCudLogic courseCudLogic,
            IUserContext userContext,
            IAccessControlContext<CourseUser> accessControlContext) : base(userContext, accessControlContext, unitOfWorkManager)
        {
            _readCourseRepository = readCourseRepository;
            _readCourseUserRepository = readCourseUserRepository;
            _courseCudLogic = courseCudLogic;
        }

        protected override async Task HandleAsync(TransferCourseOwnershipCommand command, CancellationToken cancellationToken)
        {
            var course = await _readCourseRepository.GetAsync(command.CourseId);

            EnsureValidPermission(
                course.HasSaveCourseAutomatePermission(
                    CurrentUserId,
                    CurrentUserRoles,
                    _readCourseRepository.GetHasAdminRightChecker(course, AccessControlContext)));

            EnsureBusinessLogicValid(course.ValidateNotArchived());

            var newTransferredOwner = _readCourseUserRepository.GetAll().Where(p => p.Id == command.NewOwnerId).Select(x => new { x.Email, x.PhoneNumber }).FirstOrDefault();

            course.CreatedBy = command.NewOwnerId;
            course.ChangedBy = CurrentUserIdOrDefault;
            course.MOEOfficerId = command.NewOwnerId;
            course.MOEOfficerEmail = newTransferredOwner?.Email;
            course.MOEOfficerPhoneNumber = newTransferredOwner?.PhoneNumber;

            await _courseCudLogic.Update(course, cancellationToken);
        }
    }
}
