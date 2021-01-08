using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microservice.Content.Versioning.Application.Dtos;
using Microservice.Content.Versioning.Application.Model;
using Thunder.Platform.Core.Application.Dtos;

namespace Microservice.Content.Versioning.Services
{
    public interface IVersionTrackingApplicationService
    {
        Task<PagedResultDto<VersionTrackingModel>> GetVersionTrackingByObjectId(GetVersionTrackingByObjectIdRequest dto, Guid userId);

        Task<List<VersionTrackingModel>> GetRevertableVersions(Guid originalObjectId, Guid userId);

        Task<VersionTrackingModel> GetActiveVersion(Guid originalObjectId, Guid userId);

        Task<RevertVersionResultModel> RevertVersion(RevertVersionRequest request, Guid userId);

        Task CreateVersionTracking(CreateVersionTrackingParameter createParameters);
    }
}
