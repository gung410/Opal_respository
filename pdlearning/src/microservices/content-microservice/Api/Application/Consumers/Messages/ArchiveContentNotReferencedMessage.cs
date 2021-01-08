using System;
using System.Collections.Generic;

namespace Microservice.Content.Application.Consumers
{
    public class ArchiveContentNotReferencedMessage
    {
        public List<Guid> ObjectIds { get; set; }
    }
}
