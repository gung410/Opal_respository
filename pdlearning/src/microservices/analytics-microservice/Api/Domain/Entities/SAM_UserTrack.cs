using System;

namespace Microservice.Analytics.Domain.Entities
{
    public partial class SAM_UserTrack
    {
        public Guid UserHistoryId { get; set; }

        public Guid UserId { get; set; }

        public Guid TrackId { get; set; }

        public virtual MT_Track Track { get; set; }

        public virtual SAM_UserHistory UserHistory { get; set; }
    }
}
