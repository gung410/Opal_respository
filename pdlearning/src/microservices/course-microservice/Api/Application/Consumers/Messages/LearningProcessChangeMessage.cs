using System;
using Microservice.Course.Domain.Enums;

namespace Microservice.Course.Application.Consumers
{
    public class LearningProcessChangeMessage
    {
        public Guid RegistrationId { get; set; }

        public LearningStatus LearningStatus { get; set; }

        public double? ProgressMeasure { get; set; }
    }
}
