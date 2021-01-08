using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Microservice.Content.Application.Queries;
using Microservice.Content.Versioning.Application.Commands;
using Microservice.Content.Versioning.Application.Dtos;
using Microservice.Content.Versioning.Application.Model;
using Microservice.Content.Versioning.Application.Queries;
using Microservice.Content.Versioning.Core;
using Microservice.Content.Versioning.Entities;
using Thunder.Platform.Core.Application;
using Thunder.Platform.Core.Application.Dtos;
using Thunder.Platform.Cqrs;

namespace Microservice.Content.Versioning.Services
{
    public class VersionTrackingApplicationService : ApplicationService, IVersionTrackingApplicationService
    {
        private readonly IThunderCqrs _thunderCqrs;
        private readonly IEnumerable<ICheckoutVersionResolver> _checkoutVersionResolvers;

        public VersionTrackingApplicationService(
            IThunderCqrs thunderCqrs,
            IEnumerable<ICheckoutVersionResolver> versionResolvers)
        {
            _thunderCqrs = thunderCqrs;
            _checkoutVersionResolvers = versionResolvers;
        }

        public Task<PagedResultDto<VersionTrackingModel>> GetVersionTrackingByObjectId(GetVersionTrackingByObjectIdRequest dto, Guid userId)
        {
            var searchQuery = new GetVersionTrackingByObjectIdQuery
            {
                Request = dto,
                UserId = userId,
            };

            return this._thunderCqrs.SendQuery(searchQuery);
        }

        public Task<List<VersionTrackingModel>> GetRevertableVersions(Guid originalObjectId, Guid userId)
        {
            var searchQuery = new GetRevertableVersionsQuery
            {
                OriginalObjectId = originalObjectId,
                UserId = userId,
            };

            return this._thunderCqrs.SendQuery(searchQuery);
        }

        public Task<VersionTrackingModel> GetActiveVersion(Guid originalObjectId, Guid userId)
        {
            var searchQuery = new GetActiveVersionsQuery
            {
                OriginalObjectId = originalObjectId,
                UserId = userId,
            };

            return this._thunderCqrs.SendQuery(searchQuery);
        }

        public async Task<RevertVersionResultModel> RevertVersion(RevertVersionRequest request, Guid userId)
        {
            var revertCmd = new RevertVersionCommand
            {
                Request = request,
                UserId = userId,
                NewActiveId = Guid.NewGuid()
            };

            await this._thunderCqrs.SendCommand(revertCmd);

            // Track action
            var getVersionTrackingQuery = new GetVersionTrackingByIdQuery
            {
                UserId = userId,
                VersionId = request.VersionTrackingId
            };
            var versionTracking = await _thunderCqrs.SendQuery(getVersionTrackingQuery);

            Guid versionId = Guid.NewGuid();
            await CreateVersionTracking(new CreateVersionTrackingParameter
            {
                VersionSchemaType = VersionSchemaType.DigitalContent,
                ObjectId = versionTracking.OriginalObjectId,
                UserId = userId,
                ActionComment = string.Format("Revert back to v{0}.{1}", versionTracking.MajorVersion, versionTracking.MinorVersion),
                RevertObjectId = request.CurrentActiveId,
                CanRollback = false,
                IncreaseMajorVersion = false,
                IncreaseMinorVersion = true,
                VersionId = versionId
            });

            return new RevertVersionResultModel
            {
                IsSuccess = true,
                UndoVersionId = versionId,
                NewActiveId = revertCmd.NewActiveId
            };
        }

        public async Task CreateVersionTracking(CreateVersionTrackingParameter createParameters)
        {
            var digitalContent = await _thunderCqrs.SendQuery(new GetDigitalContentByIdQuery
            {
                Id = createParameters.ObjectId,
                UserId = createParameters.UserId
            });

            var digitalContentVersionResolver = _checkoutVersionResolvers.GetCheckoutVersionResolver(createParameters.VersionSchemaType);
            var trackingCmd = new SaveVersionTrackingComand
            {
                UserId = createParameters.UserId,
                IsIncreaseMajorVersion = createParameters.IncreaseMajorVersion,
                IsIncreaseMinorVersion = createParameters.IncreaseMinorVersion,
                CreationRequest = new CreateVersionTrackingRequest
                {
                    VersionId = createParameters.VersionId,
                    ChangedByUserId = createParameters.UserId,
                    ObjectType = digitalContentVersionResolver.GetObjectType(),
                    Data = JsonSerializer.Serialize(digitalContent),
                    SchemaVersion = digitalContentVersionResolver.GetSchemaVersion(),
                    OriginalObjectId = digitalContent.OriginalObjectId == Guid.Empty ? digitalContent.Id : digitalContent.OriginalObjectId,
                    RevertObjectId = createParameters.RevertObjectId,
                    CanRollback = createParameters.CanRollback,
                    Comment = createParameters.ActionComment
                }
            };
            await _thunderCqrs.SendCommand(trackingCmd);
        }
    }
}
