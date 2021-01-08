using System;
using Thunder.Platform.Core.Domain.Auditing;

namespace Microservice.WebinarAutoscaler.Application.Models
{
    public class MeetingInfoModel
    {
        public Guid Id { get; set; }

        public string Title { get; set; }

        public string PreRecordPath { get; set; }

        /// <summary>
        /// Digital content id.
        /// </summary>
        public Guid? PreRecordId { get; set; }

        public DateTime StartTime { get; set; }

        public int ParticipantCount { get; set; }

        public Guid? BBBServerId { get; set; }

        public DateTime EndTime { get; set; }

        public bool IsLive { get; set; }

        public bool IsCanceled { get; set; }
    }
}
