using System;
using Microservice.Content.Versioning.Entities;

namespace Microservice.Content.Versioning.Application.Dtos
{
    public class RevertVersionRequest
    {
        /// <summary>
        /// The ID that used to clone.
        /// </summary>
        public Guid RevertFromRecordId { get; set; }

        /// <summary>
        /// The ID from Version tracking table.
        /// </summary>
        public Guid VersionTrackingId { get; set; }

        /// <summary>
        /// The ID of current latest version.
        /// </summary>
        public Guid CurrentActiveId { get; set; }

        public VersionSchemaType ObjectType { get; set; }
    }
}
