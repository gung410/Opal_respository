using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Microservice.StandaloneSurvey.Application.Queries;
using Microservice.StandaloneSurvey.Application.Services;
using Microservice.StandaloneSurvey.Versioning.Application.Commands;
using Microservice.StandaloneSurvey.Versioning.Application.Model;
using Microservice.StandaloneSurvey.Versioning.Application.Queries;
using Microservice.StandaloneSurvey.Versioning.Application.RequestDtos;
using Microservice.StandaloneSurvey.Versioning.Core;
using Microservice.StandaloneSurvey.Versioning.Entities;
using Thunder.Platform.Core.Application.Dtos;
using Thunder.Platform.Cqrs;

namespace Microservice.StandaloneSurvey.Versioning.Application.Services
{
    public class VersionTrackingApplicationService : BaseApplicationService
    {
        private readonly IEnumerable<ICheckoutVersionResolver> _checkoutVersionResolvers;

        public VersionTrackingApplicationService(
            IThunderCqrs thunderCqrs,
            IEnumerable<ICheckoutVersionResolver> versionResolvers) : base(thunderCqrs)
        {
            _checkoutVersionResolvers = versionResolvers;
        }

        public Task<PagedResultDto<VersionTrackingModel>> GetVersionTrackingByObjectId(GetVersionTrackingByObjectIdRequest dto, Guid userId)
        {
            var searchQuery = new GetVersionTrackingByObjectIdQuery
            {
                Request = dto,
                UserId = userId,
            };

            return this.ThunderCqrs.SendQuery(searchQuery);
        }

        public Task<List<VersionTrackingModel>> GetRevertableVersions(Guid originalObjectId, Guid userId)
        {
            var searchQuery = new GetRevertableVersionsQuery
            {
                OriginalObjectId = originalObjectId,
                UserId = userId,
            };

            return ThunderCqrs.SendQuery(searchQuery);
        }

        public Task<VersionTrackingModel> GetActiveVersion(Guid originalObjectId, Guid userId)
        {
            var searchQuery = new GetActiveVersionsQuery
            {
                OriginalObjectId = originalObjectId,
                UserId = userId,
            };

            return this.ThunderCqrs.SendQuery(searchQuery);
        }

        public async Task<RevertVersionResultModel> RevertVersion(RevertVersionRequest request, Guid userId)
        {
            var revertCmd = new RevertVersionCommand
            {
                Request = request,
                UserId = userId,
                NewActiveId = Guid.NewGuid()
            };

            await this.ThunderCqrs.SendCommand(revertCmd);

            // Track action
            var getVersionTrackingQuery = new GetVersionTrackingByIdQuery()
            {
                UserId = userId,
                VersionId = request.VersionTrackingId
            };
            var versionTracking = await ThunderCqrs.SendQuery(getVersionTrackingQuery);

            Guid versionId = Guid.NewGuid();
            await CreateVersionTracking(new CreateVersionTrackingParameter()
            {
                VersionSchemaType = VersionSchemaType.StandaloneSurvey,
                ObjectId = versionTracking.OriginalObjectId,
                UserId = userId,
                ActionComment = $"Revert back to v{versionTracking.MajorVersion}.{versionTracking.MinorVersion}",
                RevertObjectId = request.CurrentActiveId,
                CanRollback = false,
                IncreaseMajorVersion = false,
                IncreaseMinorVersion = true,
                VersionId = versionId
            });

            return new RevertVersionResultModel()
            {
                IsSuccess = true,
                UndoVersionId = versionId,
                NewActiveId = revertCmd.NewActiveId
            };
        }

        public async Task CreateVersionTracking(CreateVersionTrackingParameter createParameters)
        {
            var form = await ThunderCqrs.SendQuery(new GetVersionTrackingFormDataByIdQuery
            {
                FormId = createParameters.ObjectId,
                UserId = createParameters.UserId
            });
            var formVersionResolver = _checkoutVersionResolvers.GetCheckoutVersionResolver(createParameters.VersionSchemaType);
            var trackingCmd = new SaveVersionTrackingComand()
            {
                UserId = createParameters.UserId,
                IsIncreaseMajorVersion = createParameters.IncreaseMajorVersion,
                IsIncreaseMinorVersion = createParameters.IncreaseMinorVersion,
                CreationRequest = new CreateVersionTrackingRequest()
                {
                    VersionId = createParameters.VersionId,
                    ChangedByUserId = createParameters.UserId,
                    ObjectType = formVersionResolver.GetObjectType(),
                    Data = JsonSerializer.Serialize(form),
                    SchemaVersion = formVersionResolver.GetSchemaVersion(),
                    OriginalObjectId = form.OriginalObjectId == Guid.Empty ? form.Id : form.OriginalObjectId,
                    RevertObjectId = createParameters.RevertObjectId,
                    CanRollback = createParameters.CanRollback,
                    Comment = createParameters.ActionComment
                }
            };
            await ThunderCqrs.SendCommand(trackingCmd);
        }
    }
}
