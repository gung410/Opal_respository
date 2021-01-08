using System;
using Thunder.Platform.Cqrs;

namespace Microservice.WebinarVideoConverter.Application.Commands
{
    public class ProcessIgnoreRetryTrackingCommand : BaseThunderCommand
    {
        public Guid ConvertingTrackingId { get; set; }
    }
}
