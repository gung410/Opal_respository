using System;
using System.Collections.Generic;

namespace Microservice.Analytics.Domain.Entities
{
    public partial class MT_Track
    {
        public MT_Track()
        {
            SamUserTrack = new HashSet<SAM_UserTrack>();
        }

        public Guid TrackId { get; set; }

        public string FullStatement { get; set; }

        public string DisplayText { get; set; }

        public string GroupCode { get; set; }

        public string CodingScheme { get; set; }

        public string Note { get; set; }

        public string Type { get; set; }

        public virtual ICollection<SAM_UserTrack> SamUserTrack { get; set; }
    }
}
