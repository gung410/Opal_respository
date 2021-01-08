using System;
using System.Threading;
using System.Threading.Tasks;
using Conexus.Opal.AccessControl.Infrastructure;
using Microservice.Course.Application.BusinessLogics;
using Microservice.Course.Application.BusinessLogics.EntityCud;
using Microservice.Course.Application.Commands.Abstracts;
using Microservice.Course.Domain.Entities;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;

namespace Microservice.Course.Application.Commands.CommandHandlers
{
    public class AutoArchiveCoursesCommandHandler : BaseCommandHandler<AutoArchiveCoursesCommand>
    {
        private readonly ArchiveCourseLogic _archiveCourseLogic;
        private readonly IReadOnlyRepository<CourseEntity> _readCourseRepository;
        private readonly CourseCudLogic _courseCudLogic;

        public AutoArchiveCoursesCommandHandler(
            IUnitOfWorkManager unitOfWorkManager,
            ArchiveCourseLogic archiveCourseLogic,
            IReadOnlyRepository<CourseEntity> readCourseRepository,
            CourseCudLogic courseCudLogic,
            IUserContext userContext,
            IAccessControlContext<CourseUser> accessControlContext) : base(userContext, accessControlContext, unitOfWorkManager)
        {
            _archiveCourseLogic = archiveCourseLogic;
            _readCourseRepository = readCourseRepository;
            _courseCudLogic = courseCudLogic;
        }

        protected override async Task HandleAsync(
            AutoArchiveCoursesCommand command,
            CancellationToken cancellationToken)
        {
            var validCoursesToArchive = await _archiveCourseLogic.GetCanAutoArchiveCoursesByQuery(
                _readCourseRepository.GetAll(),
                cancellationToken);
            await _archiveCourseLogic.ArchiveCourses(validCoursesToArchive, Guid.Empty, cancellationToken);
            await _courseCudLogic.UpdateMany(validCoursesToArchive, cancellationToken);
        }
    }
}
