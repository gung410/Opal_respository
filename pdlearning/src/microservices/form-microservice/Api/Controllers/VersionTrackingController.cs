using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microservice.Form.Versioning.Application.Model;
using Microservice.Form.Versioning.Application.RequestDtos;
using Microservice.Form.Versioning.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Thunder.Platform.Core.Application.Dtos;
using Thunder.Platform.Core.Context;

namespace Microservice.Form.Controllers
{
    [Route("api/form/versioning")]
    public class VersionTrackingController : BaseController<VersionTrackingApplicationService>
    {
        public VersionTrackingController(VersionTrackingApplicationService appService, IUserContext userContext) : base(userContext, appService)
        {
        }

        [HttpPost("getVersionsByObjectId")]
        public async Task<PagedResultDto<VersionTrackingModel>> GetVersionsByObjectId([FromBody] GetVersionTrackingByObjectIdRequest request)
        {
            return await AppService.GetVersionTrackingByObjectId(request, CurrentUserId);
        }

        [HttpGet("getRevertableVersions/{originalObjectId:guid}")]
        public async Task<IEnumerable<VersionTrackingModel>> GetRevertableVesions(Guid originalObjectId)
        {
            return await AppService.GetRevertableVersions(originalObjectId, CurrentUserId);
        }

        [HttpGet("activeVersion/{originalObjectId:guid}")]
        public async Task<VersionTrackingModel> GetActiveVersion(Guid originalObjectId)
        {
            return await AppService.GetActiveVersion(originalObjectId, CurrentUserId);
        }

        [HttpPost("revertVersion")]
        public async Task<RevertVersionResultModel> RevertVersion([FromBody] RevertVersionRequest request)
        {
            return await AppService.RevertVersion(request, CurrentUserId);
        }
    }
}
