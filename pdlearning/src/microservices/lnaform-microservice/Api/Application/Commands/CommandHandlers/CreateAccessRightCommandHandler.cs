using System;
using System.Linq;
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
    public class CreateAccessRightCommandHandler : BaseCommandHandler<CreateAccessRightCommand>
    {
        private readonly IRepository<AccessRight> _accessRightRepository;
        private readonly IRepository<FormEntity> _formRepository;

        public CreateAccessRightCommandHandler(
            IRepository<AccessRight> accessRightRepository,
            IRepository<FormEntity> formRepository,
            IUnitOfWorkManager unitOfWorkManager,
            IAccessControlContext accessControlContext) : base(unitOfWorkManager, accessControlContext)
        {
            _accessRightRepository = accessRightRepository;
            _formRepository = formRepository;
        }

        protected override async Task HandleAsync(CreateAccessRightCommand command, CancellationToken cancellationToken)
        {
            var hasOwnerPermission = await _formRepository
                .GetAll()
                .ApplyAccessControlEx(
                    accessControlContext: AccessControlContext,
                    includePredicate: LnaFormEntityExpressions.HasOwnerPermissionExpr(CurrentUserId))
                .AnyAsync(
                    predicate: d => d.OriginalObjectId == command.CreationRequest.OriginalObjectId,
                    cancellationToken: cancellationToken);

            if (hasOwnerPermission)
            {
                var existedUserIds = await _accessRightRepository
                    .GetAll()
                    .Where(p => p.ObjectId == command.CreationRequest.OriginalObjectId)
                    .Select(p => p.UserId)
                    .ToListAsync(cancellationToken);

                foreach (var collaboratorId in command.CreationRequest.UserIds)
                {
                    if (!existedUserIds.Contains(collaboratorId))
                    {
                        var comment = new AccessRight
                        {
                            Id = Guid.NewGuid(),
                            UserId = collaboratorId,
                            ObjectId = command.CreationRequest.OriginalObjectId,
                            CreatedBy = command.UserId,
                        };

                        await _accessRightRepository.InsertAsync(comment);
                    }
                }
            }
        }
    }
}
