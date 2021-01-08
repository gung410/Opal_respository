using System;
using Microservice.Content.Versioning.Entities;

namespace Microservice.Content.Versioning.Application.Dtos
{
    public class CreateVersionTrackingParameter
    {
        public VersionSchemaType VersionSchemaType { get; set; }

        public Guid ObjectId { get; set; }

        public Guid UserId { get; set; }

        public string ActionComment { get; set; }

        public Guid RevertObjectId { get; set; } = default;

        public bool CanRollback { get; set; } = false;

        public bool IncreaseMajorVersion { get; set; } = false;

        public bool IncreaseMinorVersion { get; set; } = true;

        public Guid VersionId { get; set; } = default;
    }
}
