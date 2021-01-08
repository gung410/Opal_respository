using System;
using Microservice.Course.Domain.Enums;
using Thunder.Platform.Cqrs;

namespace Microservice.Course.Application.Commands
{
    public class ChangeRegistrationByLearnerCommand : BaseThunderCommand
    {
        public Guid CourseId { get; set; }

        public Guid ClassRunId { get; set; }

        public RegistrationStatus Status { get; set; }
    }
}
