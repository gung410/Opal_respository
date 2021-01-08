using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Conexus.Opal.AccessControl.Infrastructure;
using Microservice.Content.Application.Commands.CommandHandlers;
using Microservice.Content.Versioning.Entities;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;

namespace Microservice.Content.Versioning.Application.Commands
{
    public class SaveVersionTrackingCommandHandler : BaseCommandHandler<SaveVersionTrackingComand>
    {
        private readonly IRepository<VersionTracking> _versionTrackingRepository;

        public SaveVersionTrackingCommandHandler(
            IRepository<VersionTracking> versionTrackingRepository,
            IUnitOfWorkManager unitOfWorkManager,
            IUserContext userContext,
            IAccessControlContext accessControlContext) : base(unitOfWorkManager, userContext, accessControlContext)
        {
            _versionTrackingRepository = versionTrackingRepository;
        }

        protected override async Task HandleAsync(SaveVersionTrackingComand command, CancellationToken cancellationToken)
        {
            int majorVersion = 0, minorVersion = 0;

            var lastMajorVersionTracking = await _versionTrackingRepository
                .GetAll()
                .Where(v => v.OriginalObjectId == command.CreationRequest.OriginalObjectId && v.MajorVersion > 0)
                .OrderByDescending(v => v.MajorVersion)
                .Take(1)
                .FirstOrDefaultAsync(cancellationToken);

            if (command.IsIncreaseMajorVersion)
            {
                majorVersion = lastMajorVersionTracking == null ? 1 : (lastMajorVersionTracking.MajorVersion + 1);
                minorVersion = 0;
            }
            else
            {
                majorVersion = lastMajorVersionTracking == null ? 0 : lastMajorVersionTracking.MajorVersion;
                var lastMinorVersion = await _versionTrackingRepository
                    .GetAll()
                    .Where(v => v.OriginalObjectId == command.CreationRequest.OriginalObjectId)
                    .OrderByDescending(v => v.CreatedDate)
                    .Take(1)
                    .FirstOrDefaultAsync(cancellationToken);

                minorVersion = lastMinorVersion == null ? 0 : lastMinorVersion.MinorVersion;
                if (command.IsIncreaseMinorVersion)
                {
                    minorVersion++;
                }
            }

            // For versionning we do not have update action. The actions alway is create new
            var versionTracking = new VersionTracking
            {
                Id = command.CreationRequest.VersionId,
                ObjectType = command.CreationRequest.ObjectType,
                OriginalObjectId = command.CreationRequest.OriginalObjectId,
                RevertObjectId = command.CreationRequest.RevertObjectId,
                CanRollback = command.CreationRequest.CanRollback,
                ChangedByUserId = command.UserId,
                Data = command.CreationRequest.Data,
                SchemaVersion = command.CreationRequest.SchemaVersion,
                Comment = command.CreationRequest.Comment,
                MajorVersion = majorVersion,
                MinorVersion = minorVersion
            };

            await _versionTrackingRepository.InsertAsync(versionTracking);
        }
    }
}
