using System;

namespace LearnerApp.Models.PDPM
{
    public class PDOLearner
    {
        public string Id { get; set; }

        public string CourseId { get; set; }

        public string UserId { get; set; }

        public string Status { get; set; }

        public long ProgressMeasure { get; set; }

        public DateTimeOffset LastLogin { get; set; }

        public DateTimeOffset StartDate { get; set; }

        public DateTimeOffset CreatedDate { get; set; }

        public string CreatedBy { get; set; }

        public string CourseType { get; set; }

        public string ResultId { get; set; }
    }
}
