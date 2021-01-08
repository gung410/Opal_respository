using System;

namespace Microservice.Course.Application.RequestDtos
{
    public class MassNominatedRegistrationDto
    {
        public Guid LearnerId { get; set; }

        public Guid PrimaryAOId { get; set; }

        public Guid AlternativeAOId { get; set; }

        public Guid CourseId { get; set; }

        public Guid ClassRunId { get; set; }
    }
}
