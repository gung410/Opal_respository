using System.Threading;
using System.Threading.Tasks;
using Conexus.Opal.AccessControl.Extensions;
using Conexus.Opal.AccessControl.Infrastructure;
using Microservice.Content.Domain.Entities;
using Microservice.Content.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;

namespace Microservice.Content.Application.Commands.CommandHandlers
{
    public class DeleteAccessRightCommandHandler : BaseCommandHandler<DeleteAccessRightCommand>
    {
        private readonly IRepository<AccessRight> _accessRightRepository;
        private readonly IRepository<DigitalContent> _digitalContentRepository;

        public DeleteAccessRightCommandHandler(
            IRepository<AccessRight> accessRightRepository,
            IRepository<DigitalContent> digitalContentRepository,
            IUnitOfWorkManager unitOfWorkManager,
            IUserContext userContext,
            IAccessControlContext accessControlContext) : base(unitOfWorkManager, userContext, accessControlContext)
        {
            _accessRightRepository = accessRightRepository;
            _digitalContentRepository = digitalContentRepository;
        }

        protected override async Task HandleAsync(DeleteAccessRightCommand command, CancellationToken cancellationToken)
        {
            var existedAccessRights = await _accessRightRepository.FirstOrDefaultAsync(command.AccessRightId);
            if (existedAccessRights == null)
            {
                throw new ContentAccessDeniedException();
            }

            var hasOwnerPermission = await _digitalContentRepository
                .GetAllWithAccessControl(AccessControlContext, DigitalContentExpressions.HasOwnerPermissionExpr(CurrentUserId))
                .AnyAsync(d => d.OriginalObjectId == existedAccessRights.ObjectId, cancellationToken);
            if (hasOwnerPermission)
            {
                await _accessRightRepository.DeleteAsync(existedAccessRights);
            }
        }
    }
}
