using System;
using System.Linq;
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
    public class CreateAccessRightCommandHandler : BaseCommandHandler<CreateAccessRightCommand>
    {
        private readonly IRepository<AccessRight> _accessRightRepository;
        private readonly IRepository<DigitalContent> _digitalContentRepository;

        public CreateAccessRightCommandHandler(
            IRepository<AccessRight> accessRightRepository,
            IRepository<DigitalContent> digitalContentRepository,
            IUnitOfWorkManager unitOfWorkManager,
            IUserContext userContext,
            IAccessControlContext accessControlContext) : base(unitOfWorkManager, userContext, accessControlContext)
        {
            _accessRightRepository = accessRightRepository;
            _digitalContentRepository = digitalContentRepository;
        }

        protected override async Task HandleAsync(CreateAccessRightCommand command, CancellationToken cancellationToken)
        {
            var hasOwnerPermission = await _digitalContentRepository
                .GetAllWithAccessControl(AccessControlContext, DigitalContentExpressions.HasOwnerPermissionExpr(CurrentUserId))
                .AnyAsync(d => d.OriginalObjectId == command.OriginalObjectId, cancellationToken: cancellationToken);

            if (hasOwnerPermission)
            {
                var existedUserIds = _accessRightRepository
                    .GetAllList(x => x.ObjectId == command.OriginalObjectId)
                    .Select(x => x.UserId)
                    .ToList();

                foreach (var collaboratorId in command.UserIds)
                {
                    if (!existedUserIds.Contains(collaboratorId))
                    {
                        var comment = new AccessRight
                        {
                            Id = Guid.NewGuid(),
                            UserId = collaboratorId,
                            ObjectId = command.OriginalObjectId,
                            CreatedBy = command.UserId,
                        };

                        await _accessRightRepository.InsertAsync(comment);
                    }
                }
            }
        }
    }
}
