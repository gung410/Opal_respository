using System;
using System.ComponentModel.DataAnnotations;
using Thunder.Platform.Core.Domain.Auditing;

namespace Microservice.Form.Versioning.Entities
{
    public class VersionTracking : AuditedEntity
    {
        public VersionSchemaType ObjectType { get; set; }

        [Required]
        public Guid OriginalObjectId { get; set; }

        public Guid ChangedByUserId { get; set; }

        public Guid RevertObjectId { get; set; }

        public bool CanRollback { get; set; }

        public int MajorVersion { get; set; }

        public int MinorVersion { get; set; }

        [Required]
        public string SchemaVersion { get; set; }

        [Required]
        public string Data { get; set; }

        public string Comment { get; set; }
    }
}
