using System;
using JetBrains.Annotations;

namespace Microservice.Uploader.Application.RequestDtos
{
    public class ExtractScormPackageRequest
    {
        public Guid FileId { get; set; }

        public string FileExtension { get; set; }

        [CanBeNull]
        public string Folder { get; set; }
    }
}