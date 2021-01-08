using System;
using System.Collections.Generic;
using Thunder.Platform.Core.Application.Dtos;

namespace Microservice.Course.Application.RequestDtos
{
    public class MigrateCourseNotificationRequest : PagedResultRequestDto
    {
        public List<Guid> CourseIds { get; set; }
    }
}
