using System;

namespace Microservice.Course.Application.RequestDtos
{
    public class CloneContentForCourseRequest
    {
        public Guid FromCourseId { get; set; }

        public Guid ToCourseId { get; set; }
    }
}
