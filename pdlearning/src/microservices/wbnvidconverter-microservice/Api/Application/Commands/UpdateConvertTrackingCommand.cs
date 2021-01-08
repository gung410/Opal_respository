using System;
using System.Collections.Generic;
using Thunder.Platform.Cqrs;

namespace Microservice.WebinarVideoConverter.Application.Commands
{
    public class UpdateConvertTrackingCommand : BaseThunderCommand
    {
        public Guid Id { get; set; }

        public List<KeyValuePair<string, object>> Properties { get; set; }
    }
}
