using System;
using LearnerApp.Common;

namespace LearnerApp.Models.Learner
{
    public class AssignmentInfo
    {
        public string Id { get; set; }

        public string CourseId { get; set; }

        public string ClassRunId { get; set; }

        public string Title { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public bool RandomizedQuestions { get; set; }

        public AssignmentType Type { get; set; }

        public string CreatedBy { get; set; }

        public string ChangedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime? DeletedDate { get; set; }

        public DateTime? ChangedDate { get; set; }
    }
}
