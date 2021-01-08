using System;
using Microservice.Content.Versioning.Entities;

namespace Microservice.Content.Versioning.Application.Dtos
{
    public class CreateVersionTrackingRequest
    {
        public Guid VersionId
        {
            get
            {
                if (Id == Guid.Empty)
                {
                    Id = Guid.NewGuid();
                }

                return Id;
            }

            set
            {
                Id = value;
            }
        }

        public Guid OriginalObjectId { get; set; }

        public Guid RevertObjectId { get; set; }

        public Guid ChangedByUserId { get; set; }

        public VersionSchemaType ObjectType { get; set; }

        public string Data { get; set; }

        public string SchemaVersion { get; set; }

        public bool CanRollback { get; set; }

        public string Comment { get; set; }

        private Guid Id { get; set; }
    }
}
