using System;
using JetBrains.Annotations;

namespace Microservice.Uploader.Application.RequestDtos
{
    public class CompleteMultipartFileResquest
    {
        public Guid FileId { get; set; }

        public string FileExtension { get; set; }

        [CanBeNull]
        public string Folder { get; set; }

        [CanBeNull]
        public bool IsTemporary { get; set; }
    }
}
