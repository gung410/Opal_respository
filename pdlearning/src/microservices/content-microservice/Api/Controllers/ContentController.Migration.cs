using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Microservice.Content.Controllers
{
    public partial class ContentController
    {
        [HttpPost("migrate_content_notification")]
        public async Task MigrateContentNotification([FromBody] List<Guid> ids)
        {
            await _appService.MigrateContentNotification(ids);
        }
    }
}
