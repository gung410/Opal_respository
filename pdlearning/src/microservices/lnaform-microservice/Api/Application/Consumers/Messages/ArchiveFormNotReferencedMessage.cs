using System;
using System.Collections.Generic;

namespace Microservice.LnaForm.Application.Consumers
{
    public class ArchiveFormNotReferencedMessage
    {
        public List<Guid> ObjectIds { get; set; }
    }
}
