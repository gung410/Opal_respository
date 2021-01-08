using System;
using System.Collections.Generic;
using LearnerApp.Common;

namespace LearnerApp.Models.Course
{
    public class AttendanceTracking
    {
        public string Id { get; set; }

        public string SessionId { get; set; }

        public string UserId { get; set; }

        public bool IsCodeScanned { get; set; }

        public DateTime? CodeScannedDate { get; set; }

        public string ReasonForAbsence { get; set; }

        public List<string> Attachment { get; set; }

        public AttendanceTrackingStatus? Status { get; set; } = null;
    }
}
