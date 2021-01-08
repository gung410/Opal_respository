using System;
using Microservice.Course.Domain.Enums;

namespace Microservice.Course.Application.RequestDtos
{
    public class ChangeRegistrationByLearnerRequest
    {
        public Guid CourseId { get; set; }

        public Guid ClassRunId { get; set; }

        public RegistrationStatus Status { get; set; }
    }
}
