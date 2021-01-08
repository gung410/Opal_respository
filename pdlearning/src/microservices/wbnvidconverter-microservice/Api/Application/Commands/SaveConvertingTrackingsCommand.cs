using System.Collections.Generic;
using Microservice.WebinarVideoConverter.Domain.Entities;
using Thunder.Platform.Cqrs;

namespace Microservice.WebinarVideoConverter.Application.Commands
{
    public class SaveConvertingTrackingsCommand : BaseThunderCommand
    {
        public IEnumerable<ConvertingTracking> ConvertTrackings { get; set; }
    }
}
