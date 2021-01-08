using System;
using System.Collections.Generic;

#pragma warning disable SA1402 // File may only contain a single type
namespace Microservice.Calendar.Application.Consumers.Messages
{
    public class StandaloneFormPublishedMessage
    {
        public FormInfo Form { get; set; }

        public string FormUrl { get; set; }

        public List<Guid> ParticipantIds { get; set; }
    }

    public class FormInfo
    {
        public Guid OriginalObjectId { get; set; }

        public Guid OwnerId { get; set; }

        public Guid CreatedBy { get; set; }

        public string Title { get; set; }

        public string Type { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }
    }
}
#pragma warning restore SA1402 // File may only contain a single type
