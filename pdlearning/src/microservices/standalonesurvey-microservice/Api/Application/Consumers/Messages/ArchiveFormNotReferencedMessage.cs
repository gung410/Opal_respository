using System;
using System.Collections.Generic;

namespace Microservice.StandaloneSurvey.Application.Consumers
{
    public class ArchiveFormNotReferencedMessage
    {
        public List<Guid> ObjectIds { get; set; }
    }
}
