using System;
using System.Collections.Generic;

namespace Microservice.Course.Application.RequestDtos
{
    public class ArchiveCourseRequest
    {
        public List<Guid> Ids { get; set; }
    }
}
