using System.Threading;
using System.Threading.Tasks;
using Conexus.Opal.AccessControl.Extensions;
using Conexus.Opal.AccessControl.Infrastructure;
using Microservice.Course.Application.BusinessLogics.EntityCud;
using Microservice.Course.Application.Commands.Abstracts;
using Microservice.Course.Application.Constants;
using Microservice.Course.Application.SharedQueries;
using Microservice.Course.Common.Helpers;
using Microservice.Course.Domain.Entities;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;
using Thunder.Platform.Core.Exceptions;

namespace Microservice.Course.Application.Commands.CommandHandlers
{
    public class ToggleCourseCriteriaForClassRunCommandHandler : BaseCommandHandler<ToggleCourseCriteriaForClassRunCommand>
    {
        private readonly GetAggregatedClassRunSharedQuery _getAggregatedClassRunSharedQuery;
        private readonly IReadOnlyRepository<CourseEntity> _readCourseRepository;
        private readonly ClassRunCudLogic _classRunCudLogic;

        public ToggleCourseCriteriaForClassRunCommandHandler(
            IUnitOfWorkManager unitOfWorkManager,
            ClassRunCudLogic classRunCudLogic,
            IUserContext userContext,
            IAccessControlContext<CourseUser> accessControlContext,
            GetAggregatedClassRunSharedQuery getAggregatedClassRunSharedQuery,
            IReadOnlyRepository<CourseEntity> readCourseRepository) : base(userContext, accessControlContext, unitOfWorkManager)
        {
            _classRunCudLogic = classRunCudLogic;
            _getAggregatedClassRunSharedQuery = getAggregatedClassRunSharedQuery;
            _readCourseRepository = readCourseRepository;
        }

        protected override async Task HandleAsync(ToggleCourseCriteriaForClassRunCommand command, CancellationToken cancellationToken)
        {
            var aggregatedClassRun = await _getAggregatedClassRunSharedQuery.ByClassRunId(command.Id);

            EnsureValidPermission(
                aggregatedClassRun.Course.HasSaveCourseAutomatePermission(
                    CurrentUserId,
                    CurrentUserRoles,
                    _readCourseRepository.GetHasAdminRightChecker(F.List(aggregatedClassRun.Course), AccessControlContext)));

            EnsureBusinessLogicValid(aggregatedClassRun.Course.ValidateNotArchived());

            aggregatedClassRun.ClassRun.CourseCriteriaActivated = command.CourseCriteriaActivated;

            await _classRunCudLogic.Update(aggregatedClassRun, cancellationToken);
        }
    }
}
