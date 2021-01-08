using System;

namespace LearnerApp.Models
{
    public class Enroll
    {
        public string Id { get; set; }

        public string CourseId { get; set; }

        public string Version { get; set; }

        public string UserId { get; set; }

        public string Status { get; set; }

        public string ReviewStatus { get; set; }

        public int ProgressMeasure { get; set; }

        public DateTime LastLogin { get; set; }

        public DateTime DisenrollUtc { get; set; }

        public DateTime ReadDate { get; set; }

        public DateTime ReminderSentDate { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public DateTime CompletedDate { get; set; }

        public DateTime CreatedDate { get; set; }

        public string CreatedBy { get; set; }

        public DateTime ChangedDate { get; set; }

        public string ChangedBy { get; set; }
    }
}
