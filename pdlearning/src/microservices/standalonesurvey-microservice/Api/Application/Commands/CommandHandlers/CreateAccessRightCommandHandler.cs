using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Conexus.Opal.AccessControl.Infrastructure;
using Microservice.StandaloneSurvey.Domain.Entities;
using Microservice.StandaloneSurvey.Domain.ValueObjects;
using Microservice.StandaloneSurvey.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;

namespace Microservice.StandaloneSurvey.Application.Commands
{
    public class CreateAccessRightCommandHandler : BaseCommandHandler<CreateAccessRightCommand>
    {
        private readonly IRepository<AccessRight> _accessRightRepository;
        private readonly IRepository<Domain.Entities.StandaloneSurvey> _formRepository;

        public CreateAccessRightCommandHandler(
            IRepository<AccessRight> accessRightRepository,
            IRepository<Domain.Entities.StandaloneSurvey> formRepository,
            IUnitOfWorkManager unitOfWorkManager,
            IAccessControlContext accessControlContext) : base(unitOfWorkManager, accessControlContext)
        {
            _accessRightRepository = accessRightRepository;
            _formRepository = formRepository;
        }

        protected override async Task HandleAsync(CreateAccessRightCommand command, CancellationToken cancellationToken)
        {
            var formQuery = _formRepository.GetAll();

            if (command.SubModule == SubModule.Lna)
            {
                formQuery = formQuery
                    .ApplyAccessControlEx(
                        AccessControlContext,
                        SurveyEntityExpressions.HasOwnerPermissionExpr(this.CurrentUserId));
            }
            else
            {
                throw new NotSupportedFeatureException();
            }

            var hasOwnerPermission = await formQuery
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
