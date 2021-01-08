using System;
using System.Collections.Generic;
using Thunder.Platform.Core.Application.Dtos;

namespace Microservice.Course.Application.RequestDtos
{
    public class MigrateLearningRecordRequest : PagedResultRequestDto
    {
        public List<Guid> UserIds { get; set; }
    }
}
