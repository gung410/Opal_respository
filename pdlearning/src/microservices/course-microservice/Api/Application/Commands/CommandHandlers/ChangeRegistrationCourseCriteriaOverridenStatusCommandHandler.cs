using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Conexus.Opal.AccessControl.Extensions;
using Conexus.Opal.AccessControl.Infrastructure;
using Microservice.Course.Application.BusinessLogics;
using Microservice.Course.Application.BusinessLogics.EntityCud;
using Microservice.Course.Application.Commands.Abstracts;
using Microservice.Course.Application.Events;
using Microservice.Course.Application.SharedQueries;
using Microservice.Course.Common.Extensions;
using Microservice.Course.Domain.Entities;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Entities;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;
using Thunder.Platform.Core.Extensions;
using Thunder.Platform.Cqrs;

namespace Microservice.Course.Application.Commands.CommandHandlers
{
    public class ChangeRegistrationCourseCriteriaOverridenStatusCommandHandler : BaseCommandHandler<ChangeRegistrationCourseCriteriaOverridedStatusCommand>
    {
        private readonly GetAggregatedRegistrationSharedQuery _aggregatedRegistrationSharedQuery;
        private readonly RegistrationCudLogic _registrationCudLogic;
        private readonly IReadOnlyRepository<CourseEntity> _readCourseRepository;

        public ChangeRegistrationCourseCriteriaOverridenStatusCommandHandler(
            IUnitOfWorkManager unitOfWorkManager,
            IUserContext userContext,
            RegistrationCudLogic registrationCudLogic,
            IAccessControlContext<CourseUser> accessControlContext,
            GetAggregatedRegistrationSharedQuery aggregatedRegistrationSharedQuery,
            IReadOnlyRepository<CourseEntity> readCourseRepository) : base(userContext, accessControlContext, unitOfWorkManager)
        {
            _registrationCudLogic = registrationCudLogic;
            _aggregatedRegistrationSharedQuery = aggregatedRegistrationSharedQuery;
            _readCourseRepository = readCourseRepository;
        }

        protected override async Task HandleAsync(ChangeRegistrationCourseCriteriaOverridedStatusCommand command, CancellationToken cancellationToken)
        {
            var aggregatedRegistrations = await _aggregatedRegistrationSharedQuery.WithClassAndCourseByRegistrationIds(command.Ids, cancellationToken);

            var hasRegistrationCourseFullRightChecker = _readCourseRepository.GetHasAdminRightChecker(
                aggregatedRegistrations.SelectList(p => p.Course),
                AccessControlContext);
            EnsureValidPermission(aggregatedRegistrations.TrueForAll(p =>
                p.Registration.HasManagePermission(p.Course, p.ClassRun, CurrentUserId, CurrentUserRoles, hasRegistrationCourseFullRightChecker, x => HasPermissionPrefix(x))));

            aggregatedRegistrations.ForEach(p =>
            {
                p.Registration.CourseCriteriaOverrided = command.CourseCriteriaOverrided;
            });

            await _registrationCudLogic.UpdateMany(aggregatedRegistrations, cancellationToken);
        }
    }
}
