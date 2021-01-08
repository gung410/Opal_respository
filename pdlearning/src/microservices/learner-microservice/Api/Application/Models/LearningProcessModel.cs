using System;
using Microservice.Learner.Domain.ValueObject;

namespace Microservice.Learner.Application.Models
{
    public class LearningProcessModel
    {
        public Guid RegistrationId { get; set; }

        public MyCourseStatus LearningStatus { get; set; }

        public double? ProgressMeasure { get; set; }
    }
}
