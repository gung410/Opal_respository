using System;

namespace Conexus.Opal.Microservice.Tagging.Consumers
{
    public class CloneResourcePayload
    {
        public Guid CloneFromResouceId { get; set; }

        public Guid CloneToResouceId { get; set; }

        public Guid UserId { get; set; }
    }
}
