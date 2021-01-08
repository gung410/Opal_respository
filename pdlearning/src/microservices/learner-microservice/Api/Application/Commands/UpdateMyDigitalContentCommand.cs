using System;
using Microservice.Learner.Domain.ValueObject;
using Thunder.Platform.Cqrs;

namespace Microservice.Learner.Application.Commands
{
    public class UpdateMyDigitalContentCommand : BaseThunderCommand
    {
        public Guid DigitalContentId { get; set; }

        public double? ProgressMeasure { get; set; }

        public DateTime? ReadDate { get; set; }

        public MyDigitalContentStatus Status { get; set; }
    }
}
