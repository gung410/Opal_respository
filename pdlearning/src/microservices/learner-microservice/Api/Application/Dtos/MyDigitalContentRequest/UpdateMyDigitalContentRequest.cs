using System;
using Microservice.Learner.Domain.ValueObject;

namespace Microservice.Learner.Application.Dtos
{
    public class UpdateMyDigitalContentRequest
    {
        public Guid DigitalContentId { get; set; }

        public double? ProgressMeasure { get; set; }

        public DateTime? ReadDate { get; set; }

        public MyDigitalContentStatus Status { get; set; }
    }
}
