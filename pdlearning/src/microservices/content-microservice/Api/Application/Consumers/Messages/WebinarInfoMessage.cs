using System;
using System.Collections.Generic;
using Microservice.Content.Domain.Enums;

namespace Microservice.Content.Application.Consumers
{
    public class WebinarInfoMessage
    {
        public string Title { get; set; }

        public Guid Id { get; set; }

        public Guid OwnerId { get; set; }

        public IEnumerable<Guid> CollaboratorIds { get; set; }

        public ContentType Type { get; set; }

        public string FileName { get; set; }

        public string FileExtension { get; set; }

        public string FileLocation { get; set; }

        public string FileType { get; set; }

        public double FileSize { get; set; }

        public DigitalContentStatus Status { get; set; }
    }
}
