using System;
using Thunder.Platform.Cqrs;

namespace Microservice.Webinar.Application.Commands
{
    public class UpdateMeetingInfoCommand : BaseThunderCommand
    {
        public Guid Id { get; set; }

        public string Title { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        /// <summary>
        /// Relative s3 path.
        /// </summary>
        public string PreRecordPath { get; set; }

        public int ParticipantCount { get; set; }

        public Guid? PreRecordId { get; set; }
    }
}
