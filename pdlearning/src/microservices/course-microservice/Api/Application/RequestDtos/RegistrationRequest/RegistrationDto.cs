using System;

namespace Microservice.Course.Application.RequestDtos
{
    public class RegistrationDto
    {
        public Guid CourseId { get; set; }

        public Guid ClassRunId { get; set; }
    }
}
