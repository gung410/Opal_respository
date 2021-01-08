using System;
using Microservice.Webinar.Domain.Enums;

namespace Microservice.Webinar.Application.Models
{
    public class RecordModel
    {
        public Guid Id { get; set; }

        public Guid MeetingId { get; set; }

        public Guid RecordId { get; set; }

        public Guid DigitalContentId { get; set; }

        public RecordStatus Status { get; set; }
    }
}
