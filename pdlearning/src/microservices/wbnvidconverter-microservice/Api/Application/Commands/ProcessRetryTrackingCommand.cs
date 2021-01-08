using System;
using Thunder.Platform.Cqrs;

namespace Microservice.WebinarVideoConverter.Application.Commands
{
    public class ProcessRetryTrackingCommand : BaseThunderCommand
    {
        public Guid ConvertingTrackingId { get; set; }
    }
}
