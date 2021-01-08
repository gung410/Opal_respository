using System;
using Microservice.Uploader.Domain.ValueObjects;

namespace Microservice.Uploader.Application.RequestDtos
{
    public class UpdatePersonalFileRequest
    {
        public Guid Id { get; set; }

        public string FileName { get; set; }

        public FileType FileType { get; set; }

        public double FileSize { get; set; }

        public string FileExtension { get; set; }

        // This is a combination of {BucketName}/{PermanentFolder}/{FileExtension}/{Id}
        public string FileLocation { get; set; }
    }
}
