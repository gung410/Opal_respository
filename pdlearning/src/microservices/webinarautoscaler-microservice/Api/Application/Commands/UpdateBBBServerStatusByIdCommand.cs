using System;
using Microservice.WebinarAutoscaler.Domain.Enums;
using Thunder.Platform.Cqrs;

namespace Microservice.WebinarAutoscaler.Application.Commands
{
    public class UpdateBBBServerStatusByIdCommand : BaseThunderCommand
    {
        public Guid BBBServerId { get; set; }

        public BBBServerStatus Status { get; set; }
    }
}
