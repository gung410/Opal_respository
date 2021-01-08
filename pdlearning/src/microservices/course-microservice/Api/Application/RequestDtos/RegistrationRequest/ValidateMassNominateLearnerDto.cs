using System;

namespace Microservice.Course.Application.RequestDtos
{
    public class ValidateMassNominateLearnerDto
    {
        public Guid LearnerId { get; set; }

        public Guid CourseId { get; set; }
    }
}
