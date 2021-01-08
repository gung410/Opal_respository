using System;
using System.Collections.Generic;
using Thunder.Platform.Core.Application.Dtos;

namespace Microservice.Content.Application.RequestDtos
{
    public class MigrateSearchContentDataRequest : PagedResultRequestDto
    {
        public List<Guid> ContentIds { get; set; }
    }
}
