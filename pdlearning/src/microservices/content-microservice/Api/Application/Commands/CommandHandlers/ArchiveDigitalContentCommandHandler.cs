using System;
using System.Threading;
using System.Threading.Tasks;
using Conexus.Opal.AccessControl.Extensions;
using Conexus.Opal.AccessControl.Infrastructure;
using Microservice.Content.Domain.Entities;
using Microservice.Content.Domain.Enums;
using Microservice.Content.Infrastructure;
using Microservice.Content.Versioning.Application.Dtos;
using Microservice.Content.Versioning.Entities;
using Microservice.Content.Versioning.Extensions;
using Microservice.Content.Versioning.Services;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;
using Thunder.Platform.Core.Timing;

namespace Microservice.Content.Application.Commands.CommandHandlers
{
    public class ArchiveDigitalContentCommandHandler : BaseCommandHandler<ArchiveDigitalContentCommand>
    {
        private readonly IRepository<DigitalContent> _digitalContentRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly IUserContext _userContext;
        private readonly IAccessControlContext _accessControlContext;
        private readonly IVersionTrackingApplicationService _versionTrackingApplicationService;

        public ArchiveDigitalContentCommandHandler(
             IRepository<DigitalContent> digitalContentRepository,
             IUnitOfWorkManager unitOfWorkManager,
             IUserContext userContext,
             IAccessControlContext accessControlContext,
             IVersionTrackingApplicationService versionTrackingApplicationService) : base(unitOfWorkManager, userContext, accessControlContext)
        {
            _digitalContentRepository = digitalContentRepository;
            _unitOfWorkManager = unitOfWorkManager;
            _userContext = userContext;
            _accessControlContext = accessControlContext;
            _versionTrackingApplicationService = versionTrackingApplicationService;
        }

        protected override async Task HandleAsync(ArchiveDigitalContentCommand command, CancellationToken cancellationToken)
        {
            var dbQuery = _digitalContentRepository
                .GetAllWithAccessControl(AccessControlContext, DigitalContentExpressions.HasOwnerPermissionExpr(CurrentUserId))
                .IgnoreArchivedItems();

            var digitalContent = await dbQuery.FirstOrDefaultAsync(dc => dc.Id == command.ContentId, cancellationToken);

            if (digitalContent == null)
            {
                throw new ContentAccessDeniedException();
            }

            digitalContent.ArchiveDate = Clock.Now;
            digitalContent.ArchivedBy = command.ArchiveBy ?? Guid.Empty;
            digitalContent.Status = DigitalContentStatus.Archived;

            await _versionTrackingApplicationService.CreateVersionTracking(new CreateVersionTrackingParameter
            {
                VersionSchemaType = VersionSchemaType.DigitalContent,
                ObjectId = digitalContent.Id,
                UserId = CurrentUserId,
                ActionComment = "Archived",
                CanRollback = false,
                IncreaseMajorVersion = false,
                IncreaseMinorVersion = true
            });

            await _digitalContentRepository.UpdateAsync(digitalContent);
        }
    }
}
