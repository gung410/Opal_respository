using System;
using System.Collections.Generic;
using Amazon.S3.Model;
using JetBrains.Annotations;

namespace Microservice.Uploader.Application.RequestDtos
{
    public class CompleteMultipartUploadSessionRequest
    {
        public Guid FileId { get; set; }

        public string UploadId { get; set; }

        public string FileExtension { get; set; }

        [CanBeNull]
        public string Folder { get; set; }

        [CanBeNull]
        public bool IsTemporary { get; set; }

        public List<PartETag> PartETags { get; set; }
    }
}
