using System;
using LearnerApp.Common;

namespace LearnerApp.Models
{
    public class MyLecturesInfo
    {
        public string Id { get; set; }

        public string MyCourseId { get; set; }

        public string LectureId { get; set; }

        public string UserId { get; set; }

        public StatusLearning Status { get; set; }

        public string ReviewStatus { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public DateTime LastLogin { get; set; }

        public DateTime CreatedDate { get; set; }

        public string CreatedBy { get; set; }

        public DateTime ChangedDate { get; set; }

        public string ChangedBy { get; set; }
    }
}
