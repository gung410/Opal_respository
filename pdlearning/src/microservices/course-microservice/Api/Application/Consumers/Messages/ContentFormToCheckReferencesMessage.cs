using System;
using System.Collections.Generic;
using Microservice.Course.Application.Events;

namespace Microservice.Course.Application.Consumers
{
    public class ContentFormToCheckReferencesMessage
    {
        public List<Guid> ObjectIds { get; set; }

        public ResourcesNotReferencedType ContentType { get; set; }
    }
}
