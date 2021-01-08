using System;
using Thunder.Platform.Cqrs;

namespace Microservice.WebinarAutoscaler.Application.Commands
{
    public class CreateMeetingInfoCommand : BaseThunderCommand
    {
        public Guid Id { get; set; }

        public string Title { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public string PreRecordPath { get; set; }

        public Guid? PreRecordId { get; set; }

        public int ParticipantCount { get; set; }

        public bool IsCanceled { get; set; }
    }
}
