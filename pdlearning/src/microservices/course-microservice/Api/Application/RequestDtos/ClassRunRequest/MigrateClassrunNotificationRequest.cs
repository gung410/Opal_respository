using System;
using System.Collections.Generic;
using Thunder.Platform.Core.Application.Dtos;

namespace Microservice.Course.Application.RequestDtos
{
    public class MigrateClassrunNotificationRequest : PagedResultRequestDto
    {
        public List<Guid> ClassRunIds { get; set; }
    }
}
