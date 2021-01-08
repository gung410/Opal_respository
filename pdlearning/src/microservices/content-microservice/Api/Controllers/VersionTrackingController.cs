using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microservice.Content.Versioning.Application.Dtos;
using Microservice.Content.Versioning.Application.Model;
using Microservice.Content.Versioning.Services;
using Microsoft.AspNetCore.Mvc;
using Thunder.Platform.AspNetCore.Controllers;
using Thunder.Platform.Core.Application.Dtos;
using Thunder.Platform.Core.Context;

namespace Microservice.Content.Controllers
{
    [Route("api/content/versioning")]
    public class VersionTrackingController : ApplicationApiController
    {
        private readonly IVersionTrackingApplicationService _appService;

        public VersionTrackingController(IVersionTrackingApplicationService appService, IUserContext userContext) : base(userContext)
        {
            _appService = appService;
        }

        [HttpPost("getVersionsByObjectId")]
        public async Task<PagedResultDto<VersionTrackingModel>> GetVersionsByObjectId([FromBody] GetVersionTrackingByObjectIdRequest request)
        {
            return await _appService.GetVersionTrackingByObjectId(request, CurrentUserId);
        }

        [HttpGet("getRevertableVersions/{originalObjectId:guid}")]
        public async Task<IEnumerable<VersionTrackingModel>> GetRevertableVersions(Guid originalObjectId)
        {
            return await _appService.GetRevertableVersions(originalObjectId, CurrentUserId);
        }

        [HttpGet("activeVersion/{originalObjectId:guid}")]
        public async Task<VersionTrackingModel> GetActiveVersion(Guid originalObjectId)
        {
            return await _appService.GetActiveVersion(originalObjectId, CurrentUserId);
        }

        [HttpPost("revertVersion")]
        public async Task<RevertVersionResultModel> RevertVersion([FromBody] RevertVersionRequest request)
        {
            return await _appService.RevertVersion(request, CurrentUserId);
        }
    }
}
