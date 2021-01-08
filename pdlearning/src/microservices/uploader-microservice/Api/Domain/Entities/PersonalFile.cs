using System;
using Microservice.Uploader.Domain.ValueObjects;
using Thunder.Platform.Core.Domain.Auditing;

namespace Microservice.Uploader.Domain.Entities
{
    public class PersonalFile : AuditedEntity
    {
        public Guid UserId { get; set; }

        public Guid PersonalSpaceId { get; set; }

        public string FileName { get; set; }

        public FileType FileType { get; set; }

        public string FileExtension { get; set; }

        public double FileSize { get; set; }

        // This is a combination of {BucketName}/{PermanentFolder}/{FileExtension}/{Id}
        public string FileLocation { get; set; }
    }
}
