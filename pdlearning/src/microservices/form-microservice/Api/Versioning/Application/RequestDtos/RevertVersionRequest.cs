using System;
using Microservice.Form.Versioning.Entities;

namespace Microservice.Form.Versioning.Application.RequestDtos
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
