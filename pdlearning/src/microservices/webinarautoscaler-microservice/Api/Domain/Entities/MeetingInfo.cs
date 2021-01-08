using System;
using Thunder.Platform.Core.Domain.Auditing;

namespace Microservice.WebinarAutoscaler.Domain.Entities
{
    public class MeetingInfo : AuditedEntity
    {
        public string Title { get; set; }

        public string PreRecordPath { get; set; }

        /// <summary>
        /// Digital content id.
        /// </summary>
        public Guid? PreRecordId { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        /// <summary>
        /// Number of meeting participants.
        /// </summary>
        public int ParticipantCount { get; set; }

        public Guid? BBBServerId { get; set; }

        /// <summary>
        /// Meeting is canceled.
        /// </summary>
        public bool IsCanceled { get; set; }

        /// <summary>
        /// Meeting is running on Server.
        /// </summary>
        public bool IsLive { get; set; }
    }
}
