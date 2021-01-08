using System;
using Microservice.Webinar.Domain.Enums;
using Thunder.Platform.Cqrs;

namespace Microservice.Webinar.Application.Commands
{
    public class SaveRecordCommand : BaseThunderCommand
    {
        public Guid Id { get; set; }

        public Guid MeetingId { get; set; }

        public string InternalMeetingId { get; set; }

        public Guid RecordId { get; set; }

        public RecordStatus Status { get; set; }
    }
}
