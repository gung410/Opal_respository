using System;
using Microservice.WebinarAutoscaler.Domain.Enums;
using Thunder.Platform.Cqrs;

namespace Microservice.WebinarAutoscaler.Application.Commands
{
    public class UpdateBBBServerProtectionStateByIdCommand : BaseThunderCommand
    {
        public Guid BBBServerId { get; set; }

        public bool IsProtection { get; set; }
    }
}
