using System;
using JetBrains.Annotations;

namespace Microservice.Uploader.Dtos
{
    public class CreateMultipartUploadSessionRequest
    {
        public Guid FileId { get; set; }

        public string FileExtension { get; set; }

        [CanBeNull]
        public string Folder { get; set; }

        [CanBeNull]
        public Guid UserId { get; set; }

        [CanBeNull]
        public double FileSize { get; set; }
    }
}
