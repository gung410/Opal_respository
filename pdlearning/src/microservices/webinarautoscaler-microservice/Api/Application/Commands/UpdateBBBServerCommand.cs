using System.Collections.Generic;
using Microservice.WebinarAutoscaler.Application.Models;
using Thunder.Platform.Cqrs;

namespace Microservice.WebinarAutoscaler.Application.Commands
{
    public class UpdateBBBServerCommand : BaseThunderCommand
    {
        public BBBServerModel BBBServer { get; set; }
    }
}
