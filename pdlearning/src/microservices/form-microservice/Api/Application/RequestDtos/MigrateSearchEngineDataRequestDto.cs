using System;
using System.Collections.Generic;
using Thunder.Platform.Core.Application.Dtos;

namespace Microservice.Form.Application.RequestDtos
{
    public class MigrateSearchEngineDataRequestDto : PagedResultRequestDto
    {
        public List<Guid> FormIds { get; set; }
    }
}
