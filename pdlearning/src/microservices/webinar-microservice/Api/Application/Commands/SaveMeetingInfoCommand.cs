using System;
using Thunder.Platform.Cqrs;

namespace Microservice.Webinar.Application.Commands
{
    public class SaveMeetingInfoCommand : BaseThunderCommand
    {
        public Guid Id { get; set; }

        public string Title { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public int ParticipantCount { get; set; }

        /// <summary>
        /// Relative s3 path.
        /// </summary>
        public string PreRecordPath { get; set; }

        public Guid? PreRecordId { get; set; }

        public bool IsCanceled { get; set; }
    }
}
