using System;
using Thunder.Platform.Core.Domain.Auditing;

namespace Microservice.Webinar.Domain.Entities
{
    public class MeetingInfo : AuditedEntity
    {
        public string Title { get; set; }

        public string PreRecordPath { get; set; }

        public string BBBServerPrivateIp { get; set; }

        /// <summary>
        /// Digital content id.
        /// </summary>
        public Guid? PreRecordId { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public bool IsCanceled { get; set; }
    }
}
