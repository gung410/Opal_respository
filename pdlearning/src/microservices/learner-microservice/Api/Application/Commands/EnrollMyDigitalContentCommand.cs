using System;
using Thunder.Platform.Cqrs;

namespace Microservice.Learner.Application.Commands
{
    public class EnrollMyDigitalContentCommand : BaseThunderCommand
    {
        public Guid Id { get; } = Guid.NewGuid();

        public Guid DigitalContentId { get; set; }
    }
}
