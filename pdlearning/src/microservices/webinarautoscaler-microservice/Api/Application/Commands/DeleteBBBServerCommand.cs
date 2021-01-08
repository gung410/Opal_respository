using System;
using Thunder.Platform.Cqrs;

namespace Microservice.WebinarAutoscaler.Application.Commands
{
    public class DeleteBBBServerCommand : BaseThunderCommand
    {
        public Guid BBBServerId { get; set; }
    }
}
