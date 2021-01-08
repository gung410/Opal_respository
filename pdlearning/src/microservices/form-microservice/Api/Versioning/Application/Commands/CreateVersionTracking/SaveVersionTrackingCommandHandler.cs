using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Conexus.Opal.AccessControl.Infrastructure;
using Microservice.Form.Application.Commands;
using Microservice.Form.Versioning.Entities;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;

namespace Microservice.Form.Versioning.Application.Commands
{
    public class SaveVersionTrackingCommandHandler : BaseCommandHandler<SaveVersionTrackingComand>
    {
        private readonly IRepository<VersionTracking> _versionTrackingRepository;

        public SaveVersionTrackingCommandHandler(
            IAccessControlContext accessControlContext,
            IRepository<VersionTracking> versionTrackingRepository,
            IUnitOfWorkManager unitOfWorkManager) : base(unitOfWorkManager, accessControlContext)
        {
            _versionTrackingRepository = versionTrackingRepository;
        }

        protected override async Task HandleAsync(SaveVersionTrackingComand command, CancellationToken cancellationToken)
        {
            int majorVersion = 0, minorVersion = 0;

            var lastMajorVersionTracking = await _versionTrackingRepository.GetAll()
                .Where(v => v.OriginalObjectId == command.CreationRequest.OriginalObjectId && v.MajorVersion > 0)
                .OrderByDescending(v => v.MajorVersion)
                .FirstOrDefaultAsync();

            if (command.IsIncreaseMajorVersion)
            {
                majorVersion = lastMajorVersionTracking == null ? 1 : (lastMajorVersionTracking.MajorVersion + 1);
                minorVersion = 0;
            }
            else
            {
                majorVersion = lastMajorVersionTracking == null ? 0 : lastMajorVersionTracking.MajorVersion;
                var lastMinorVersion = await _versionTrackingRepository.GetAll()
                    .Where(v => v.OriginalObjectId == command.CreationRequest.OriginalObjectId)
                    .OrderByDescending(v => v.CreatedDate)
                    .FirstOrDefaultAsync();
                minorVersion = lastMinorVersion == null ? 0 : lastMinorVersion.MinorVersion;
                if (command.IsIncreaseMinorVersion)
                {
                    minorVersion++;
                }
            }

            // For versionning we do not have update action. The actions alway is create new
            var versionTracking = new VersionTracking()
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
