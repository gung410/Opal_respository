using System;
using System.Collections.Generic;
using Microservice.Course.Domain.Enums;
using Thunder.Platform.Cqrs;

namespace Microservice.Course.Application.Commands
{
    public class UpdateRegistrationLearningInfoCommand : BaseThunderCommand
    {
        public List<Guid> RegistrationIds { get; set; }

        public double? LearningContentProgress { get; set; }

        public LearningStatus LearningStatus { get; set; }

        public Guid? ClassRunId { get; set; }
    }
}
