using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microservice.Form.Application.Events.EventPayloads
{
    public abstract class BaseTodoEventPayload
    {
        public string ActionName { get; set; } = string.Empty;

        public string ActionUrl { get; set; } = string.Empty;

        public string ObjectType { get; set; }

        public Guid ObjectId { get; set; } = Guid.Empty;
    }
}
