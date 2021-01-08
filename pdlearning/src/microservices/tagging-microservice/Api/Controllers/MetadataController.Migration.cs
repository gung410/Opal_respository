using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Conexus.Opal.Microservice.Tagging.Services;
using Microsoft.AspNetCore.Mvc;
using Thunder.Platform.AspNetCore.Controllers;
using Thunder.Platform.Core.Context;

namespace Conexus.Opal.Microservice.Tagging.Controllers
{
    [Route("api")]
    public class MetadataMigrationController : ApplicationApiController
    {
        private readonly ResourceService _resourceService;

        public MetadataMigrationController(IUserContext userContext, ResourceService resourceService) : base(userContext)
        {
            _resourceService = resourceService;
        }

        [HttpPost("metadataTag/migrate_tags_notification")]
        public async Task MigrateTagsNotification([FromBody] List<Guid> ids)
        {
            await _resourceService.MigrateTagsNotification(ids, CurrentUserId);
        }
    }
}
