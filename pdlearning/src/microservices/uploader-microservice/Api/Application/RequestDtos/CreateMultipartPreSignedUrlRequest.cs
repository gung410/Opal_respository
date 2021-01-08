using System;
using JetBrains.Annotations;

namespace Microservice.Uploader.Application.RequestDtos
{
    public class CreateMultipartPreSignedUrlRequest
    {
        public Guid FileId { get; set; }

        public string UploadId { get; set; }

        public string FileExtension { get; set; }

        [CanBeNull]
        public string Folder { get; set; }

        public int PartNumber { get; set; }
    }
}
