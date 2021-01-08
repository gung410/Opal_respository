using System;
using System.Collections.Generic;

namespace Microservice.Form.Application.Consumers
{
    public class ArchiveFormNotReferencedMessage
    {
        public List<Guid> ObjectIds { get; set; }
    }
}
