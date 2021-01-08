using System;
using System.Collections.Generic;
using Thunder.Platform.Cqrs;

namespace Microservice.Content.Application.Commands
{
    public class MigrateContentNotificationCommand : BaseThunderCommand
    {
        public List<Guid> ListIds { get; set; }
    }
}
