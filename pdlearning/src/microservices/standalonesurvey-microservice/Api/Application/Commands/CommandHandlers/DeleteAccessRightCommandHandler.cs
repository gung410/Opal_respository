using System.Threading;
using System.Threading.Tasks;
using Conexus.Opal.AccessControl.Infrastructure;
using Microservice.StandaloneSurvey.Domain.Entities;
using Microservice.StandaloneSurvey.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;

namespace Microservice.StandaloneSurvey.Application.Commands
{
    public class DeleteAccessRightCommandHandler : BaseCommandHandler<DeleteAccessRightCommand>
    {
        private readonly IRepository<AccessRight> _accessRightRepository;
        private readonly IRepository<Domain.Entities.StandaloneSurvey> _formRepository;

        public DeleteAccessRightCommandHandler(
            IRepository<AccessRight> accessRightRepository,
            IRepository<Domain.Entities.StandaloneSurvey> formRepository,
            IUnitOfWorkManager unitOfWorkManager,
            IAccessControlContext accessControlContext) : base(unitOfWorkManager, accessControlContext)
        {
            _accessRightRepository = accessRightRepository;
            _formRepository = formRepository;
        }

        protected override async Task HandleAsync(DeleteAccessRightCommand command, CancellationToken cancellationToken)
        {
            if (command.SubModule == SubModule.Csl)
            {
                throw new NotSupportedFeatureException();
            }

            var existedAccessRights = await _accessRightRepository.GetAsync(command.Id);
            if (existedAccessRights == null)
            {
                throw new SurveyAccessDeniedException();
            }

            var hasOwnerPermission = await _formRepository
                .GetAll()
                .ApplyAccessControlEx(
                    accessControlContext: AccessControlContext,
                    includePredicate: SurveyEntityExpressions.HasOwnerPermissionExpr(CurrentUserId))
                .AnyAsync(
                    predicate: p => p.OriginalObjectId == existedAccessRights.ObjectId,
                    cancellationToken: cancellationToken);

            if (hasOwnerPermission)
            {
                await _accessRightRepository.DeleteAsync(existedAccessRights);
            }
        }
    }
}
