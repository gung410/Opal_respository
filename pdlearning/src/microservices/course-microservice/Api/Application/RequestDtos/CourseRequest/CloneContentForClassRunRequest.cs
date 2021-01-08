using System;

namespace Microservice.Course.Application.RequestDtos
{
    public class CloneContentForClassRunRequest
    {
        public Guid CourseId { get; set; }

        public Guid ClassRunId { get; set; }
    }
}
