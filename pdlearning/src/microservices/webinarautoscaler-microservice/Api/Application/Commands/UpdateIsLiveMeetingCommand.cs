using System;
using Thunder.Platform.Cqrs;

namespace Microservice.WebinarAutoscaler.Application.Commands
{
    public class UpdateIsLiveMeetingCommand : BaseThunderCommand
    {
        public Guid Id { get; set; }

        public bool IsLive { get; set; }
    }
}
