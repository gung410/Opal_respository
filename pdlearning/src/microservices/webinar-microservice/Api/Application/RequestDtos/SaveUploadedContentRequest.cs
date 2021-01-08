using System;
using System.Collections.Generic;

namespace Microservice.Webinar.Application.RequestDtos
{
    public class SaveUploadedContentRequest
    {
        public Guid Id { get; set; }

        public Guid OwnerId { get; set; }

        public List<Guid> CollaboratorIds { get; set; }

        public string FileExtension { get; set; }

        public string FileLocation { get; set; }

        public string FileName { get; set; }

        public double FileSize { get; set; }

        public string FileType { get; set; }

        public string Title { get; set; }

        public string Type { get; set; }

        public string Status { get; set; }
    }
}
