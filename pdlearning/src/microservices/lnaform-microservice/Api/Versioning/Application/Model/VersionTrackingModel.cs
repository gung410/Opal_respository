using System;
using Microservice.LnaForm.Versioning.Entities;

namespace Microservice.LnaForm.Versioning.Application.Model
{
    public class VersionTrackingModel
    {
        public Guid Id { get; set; }

        public VersionSchemaType ObjectType { get; set; }

        public Guid OriginalObjectId { get; set; }

        public bool CanRollback { get; set; }

        public Guid RevertObjectId { get; set; }

        public Guid ChangedByUserId { get; set; }

        public int MajorVersion { get; set; }

        public int MinorVersion { get; set; }

        public string Comment { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}
