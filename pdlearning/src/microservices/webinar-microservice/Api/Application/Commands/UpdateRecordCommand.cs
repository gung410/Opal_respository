using System;
using Microservice.Webinar.Domain.Enums;
using Thunder.Platform.Cqrs;

namespace Microservice.Webinar.Application.Commands
{
    public class UpdateRecordCommand : BaseThunderCommand
    {
        public Guid MeetingId { get; set; }

        public string InternalMeetingId { get; set; }

        public RecordStatus Status { get; set; }
    }
}
