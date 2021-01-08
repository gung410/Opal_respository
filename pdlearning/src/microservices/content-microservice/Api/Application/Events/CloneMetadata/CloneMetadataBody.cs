using System;

namespace Microservice.Content.Application.Events
{
    public class CloneMetadataBody
    {
        public Guid CloneFromResouceId { get; set; }

        public Guid CloneToResouceId { get; set; }

        public Guid UserId { get; set; }
    }
}
