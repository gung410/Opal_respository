using System.Threading;
using System.Threading.Tasks;
using Conexus.Opal.AccessControl.Extensions;
using Conexus.Opal.AccessControl.Infrastructure;
using Microservice.LnaForm.Domain.Entities;
using Microservice.LnaForm.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;
using FormEntity = Microservice.LnaForm.Domain.Entities.Form;

namespace Microservice.LnaForm.Application.Commands
{
    public class DeleteAccessRightCommandHandler : BaseCommandHandler<DeleteAccessRightCommand>
    {
        private readonly IRepository<AccessRight> _accessRightRepository;
        private readonly IRepository<FormEntity> _formRepository;

        public DeleteAccessRightCommandHandler(
            IRepository<AccessRight> accessRightRepository,
            IRepository<FormEntity> formRepository,
            IUnitOfWorkManager unitOfWorkManager,
            IAccessControlContext accessControlContext) : base(unitOfWorkManager, accessControlContext)
        {
            _accessRightRepository = accessRightRepository;
            _formRepository = formRepository;
        }

        protected override async Task HandleAsync(DeleteAccessRightCommand command, CancellationToken cancellationToken)
        {
            var existedAccessRights = await _accessRightRepository.GetAsync(command.Id);
            if (existedAccessRights == null)
            {
                throw new FormAccessDeniedException();
            }

            var hasOwnerPermission = await _formRepository
                .GetAll()
                .ApplyAccessControlEx(
                    accessControlContext: AccessControlContext,
                    includePredicate: LnaFormEntityExpressions.HasOwnerPermissionExpr(CurrentUserId))
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
