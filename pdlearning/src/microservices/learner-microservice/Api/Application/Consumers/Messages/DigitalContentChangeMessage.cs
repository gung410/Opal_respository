using System;
using Microservice.Learner.Domain.ValueObject;

namespace Microservice.Learner.Application.Consumers
{
    public class DigitalContentChangeMessage
    {
        public Guid Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public ContentType Type { get; set; }

        public DigitalContentStatus Status { get; set; }

        public Guid OriginalObjectId { get; set; }

        public Guid OwnerId { get; set; }

        public DateTime? ChangedDate { get; set; }

        public DateTime CreatedDate { get; set; }

        public string FileExtension { get; set; }
    }
}
