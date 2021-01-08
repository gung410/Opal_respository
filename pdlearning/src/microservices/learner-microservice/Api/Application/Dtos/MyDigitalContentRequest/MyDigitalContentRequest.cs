using System;
using Microservice.Learner.Domain.ValueObject;

namespace Microservice.Learner.Application.Dtos
{
    public class MyDigitalContentRequest
    {
        public Guid DigitalContentId { get; set; }

        public MyDigitalContentStatus Status { get; set; }

        public DigitalContentType DigitalContentType { get; set; }

        public double? ProgressMeasure { get; set; }
    }
}
